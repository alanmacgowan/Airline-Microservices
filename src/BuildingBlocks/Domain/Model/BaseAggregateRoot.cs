﻿using System.Collections.Immutable;
using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain.Model
{
    public abstract class BaseAggregateRoot<TId> : Entity<TId>, IAggregate
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
            => _domainEvents?.Remove(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents?.Clear();
    }
}
