using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain;
using Flight.Airport.Dtos;
using Flight.Airport.Exceptions;
using Flight.Data;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Airport.Features.CreateAirport;

public class CreateAirportCommandHandler : IRequestHandler<CreateAirportCommand, AirportResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public CreateAirportCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<AirportResponseDto> Handle(CreateAirportCommand command, CancellationToken cancellationToken)
    {
        var airport = await _flightDbContext.Airports.SingleOrDefaultAsync(x => x.Code == command.Code, cancellationToken);

        if (airport is not null)
            throw new AirportAlreadyExistException();

        var airportEntity = Models.Airport.Create(command.Name, command.Code, command.Address);

        var newAirport = await _flightDbContext.Airports.AddAsync(airportEntity, cancellationToken);

        await _flightDbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AirportResponseDto>(newAirport.Entity);
    }
}
