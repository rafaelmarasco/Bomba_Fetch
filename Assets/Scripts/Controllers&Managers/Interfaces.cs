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
public interface ICanvasTool : IHeldTool
{
    public abstract void DisableCanvas();
}

public interface IHideable
{
    public abstract void Hide();
    public abstract void Reveal();
}