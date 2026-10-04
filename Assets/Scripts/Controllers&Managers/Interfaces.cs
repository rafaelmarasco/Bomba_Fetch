public interface IUsable
{
    public abstract void Use();

    void StopUsing() { }
}

public interface IPickable //Provavelmente desnecessario mas vou deixar aqui sla
{
    public abstract void Pickup();

    public abstract void Drop();
}

public interface IHeldTool : IUsable, IPickable { }