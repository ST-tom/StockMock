using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StockMock.Core;
using TS.Shared.User;

namespace StockMock.Repository
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly IUser _user;

        public AuditableEntityInterceptor(
            IUser user)
        {
            _user = user;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context!);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context!);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext context)
        {
            if (context == null) 
                return;

            foreach (var entry in context.ChangeTracker.Entries<BaseAuditEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    var now = DateTime.Now;
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.Creator = _user.Id;
                        entry.Entity.CreatorName = _user.Name;
                        entry.Entity.CreateTime = DateTime.Now;
                    }
                    entry.Entity.Updator = _user.Id;
                    entry.Entity.UpdatorName = _user.Name;
                    entry.Entity.UpdateTime = now;
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
