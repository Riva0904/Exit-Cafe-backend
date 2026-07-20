using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Customers;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<CustomerDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default)
    {
        IQueryable<Customer> q = _uow.Customers.Query().Include(c => c.Addresses).Include(c => c.Orders);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            q = q.Where(c => c.FirstName.Contains(query.SearchTerm) || c.LastName.Contains(query.SearchTerm) || c.Email.Contains(query.SearchTerm));

        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(c => c.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<CustomerDto>
        {
            Items = _mapper.Map<List<CustomerDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await _uow.Customers.Query().Include(c => c.Addresses).Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(Customer), id);
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var customer = await _uow.Customers.Query().Include(c => c.Addresses).Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(Customer), userId);
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerAddressDto> AddAddressAsync(Guid customerId, CreateAddressRequest request, CancellationToken ct = default)
    {
        if (!await _uow.Customers.AnyAsync(c => c.Id == customerId, ct))
            throw new NotFoundException(nameof(Customer), customerId);

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            IsDefault = request.IsDefault
        };

        await _uow.CustomerAddresses.AddAsync(address, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<CustomerAddressDto>(address);
    }

    public async Task<List<CustomerAddressDto>> GetAddressesAsync(Guid customerId, CancellationToken ct = default)
    {
        var addresses = await _uow.CustomerAddresses.Query()
            .Where(a => a.CustomerId == customerId).ToListAsync(ct);
        return _mapper.Map<List<CustomerAddressDto>>(addresses);
    }

    public async Task DeleteAddressAsync(Guid customerId, Guid addressId, CancellationToken ct = default)
    {
        var address = await _uow.CustomerAddresses.Query()
            .FirstOrDefaultAsync(a => a.Id == addressId && a.CustomerId == customerId, ct)
            ?? throw new NotFoundException(nameof(CustomerAddress), addressId);

        _uow.CustomerAddresses.Remove(address);
        await _uow.SaveChangesAsync(ct);
    }
}
