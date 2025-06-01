namespace HWT.Domain.Entities;

public class UexWrapper<T>
{
  
    public string Status { get; set; } = default!;

    public int HttpCode { get; set; }

    public T Data { get; set; } = default!;
}