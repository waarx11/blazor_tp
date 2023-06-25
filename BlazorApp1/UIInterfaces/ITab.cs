using Microsoft.AspNetCore.Components;

namespace BlazorApp1.UIInterfaces
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}