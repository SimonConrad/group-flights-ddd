using GroupFlights.Postsale.Domain.Changes.Outcome;
using GroupFlights.Postsale.Domain.Changes.Payments;
using GroupFlights.Postsale.Domain.Changes.Request;
using GroupFlights.Postsale.Domain.Shared;
using GroupFlights.Shared.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupFlights.Postsale.Infrastructure.Data.EF.Configs;

internal class ReservationChangeRequestConfig : IEntityTypeConfiguration<ReservationChangeRequest>
{
    // tabela ChangeRequests
    public void Configure(EntityTypeBuilder<ReservationChangeRequest> builder)
    {
        builder.HasKey(x => x.Id);
        
        // HasOne - mapuje na klucz obcy tutaj w ChangeRequests._reservationToChangeReservationId
        // wskazuje na  ReservationSnapshots.ReservationId
        // relacja do tabeli ReservationSnapshots
        builder.HasOne<ReservationToChange>("_reservationToChange");
        builder.Property<DateTime>("_newTravelDate"); // sposób na mapowanie prywatnych pol - przekazanie po nazwie w strinu
        
        // Has Conversion
        builder.Property<UserId>("_requester")
            .HasConversion<Guid?>(x => x == null ? null : x.Value, 
                x => x == null ? null : new (x.Value))
            .IsRequired();

        builder.Property<bool>("_isFeasible");
        
        // OwnsOne - mapuje pola z typu RequiredPayment na kolumny w tabeli
        builder.OwnsOne<RequiredPayment>("_paymentRequiredToApplyChange", p =>
        {
            p.OwnsOne(x => x.Deadline);
        });
        
        // OwnsOne - jak wyżej
        builder.OwnsOne<ReservationCost>("_newCost",
            c =>
            {
                c.OwnsOne(x => x.TotalCost, m =>
                {
                    m.Property(x => x.Amount).IsRequired();
                    m.Property(x => x.Currency).IsRequired();
                });
                c.Navigation(x => x.TotalCost).IsRequired();
                
                c.OwnsOne(x => x.RefundableCost, m =>
                {
                    m.Property(x => x.Amount).IsRequired();
                    m.Property(x => x.Currency).IsRequired();
                });
                c.Navigation(x => x.RefundableCost).IsRequired();
                
            }).Navigation("_newCost").IsRequired(false);
        
        // OwnsMany mapuje na nową tabele ChangeRequests_newTravel
        builder.OwnsMany<FlightSegment>("_newTravel", t =>
        {
            t.OwnsOne(x => x.FlightTime);
            t.Property(x => x.Date).IsRequired();
            t.OwnsOne(x => x.SourceAirport, a =>
            {
                a.Property(x => x.Code)
                    .HasConversion(x => x.Value,
                        x => new(x))
                    .IsRequired();
            });
            t.OwnsOne(x => x.TargetAirport, a =>
            {
                a.Property(x => x.Code)
                    .HasConversion(x => x.Value,
                        x => new(x))
                    .IsRequired();
            });
            t.OwnsOne(x => x.FlightTime);

        });

        // HasOne mapuje na klucz obcy tutaj  w ChangeRequests._changeToApplyId
        // relacja do tabeli ChangesToApply
        builder.HasOne<ReservationChangeToApply>("_changeToApply");

        builder.Property<bool>("_isActive");
        builder.Property<ReservationChangeRequest.CompletionStatus?>("_completionStatus");
    }
}