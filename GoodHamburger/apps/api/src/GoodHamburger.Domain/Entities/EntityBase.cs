namespace GoodHamburger.Domain.Entities;
public class EntityBase {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Soft delete: rows are never physically removed by the application.
    // A global EF query filter hides deleted rows from every query.
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public EntityBase() {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkDeleted() {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore() {
        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
