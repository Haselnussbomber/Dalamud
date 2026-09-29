using System.Collections.Generic;

using Dalamud.IoC;
using Dalamud.IoC.Internal;

namespace Dalamud.Services.NamePlateGui;

/// <summary>
/// Plugin-scoped version of a AddonEventManager service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<INamePlateGui>]
#pragma warning restore SA1015
internal class NamePlateGuiPluginScoped : IInternalDisposableService, INamePlateGui
{
    [ServiceManager.ServiceDependency]
    private readonly NamePlateGui parentService = Service<NamePlateGui>.Get();

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? NamePlateUpdate
    {
        add
        {
            if (this.OnNamePlateUpdateScoped == null)
                this.parentService.NamePlateUpdate += this.OnNamePlateUpdateForward;

            this.OnNamePlateUpdateScoped += value;
        }

        remove
        {
            this.OnNamePlateUpdateScoped -= value;
            if (this.OnNamePlateUpdateScoped == null)
                this.parentService.NamePlateUpdate -= this.OnNamePlateUpdateForward;
        }
    }

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? PostNamePlateUpdate
    {
        add
        {
            if (this.OnPostNamePlateUpdateScoped == null)
                this.parentService.PostNamePlateUpdate += this.OnPostNamePlateUpdateForward;

            this.OnPostNamePlateUpdateScoped += value;
        }

        remove
        {
            this.OnPostNamePlateUpdateScoped -= value;
            if (this.OnPostNamePlateUpdateScoped == null)
                this.parentService.PostNamePlateUpdate -= this.OnPostNamePlateUpdateForward;
        }
    }

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? DataUpdate
    {
        add
        {
            if (this.OnDataUpdateScoped == null)
                this.parentService.DataUpdate += this.OnDataUpdateForward;

            this.OnDataUpdateScoped += value;
        }

        remove
        {
            this.OnDataUpdateScoped -= value;
            if (this.OnDataUpdateScoped == null)
                this.parentService.DataUpdate -= this.OnDataUpdateForward;
        }
    }

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? PostDataUpdate
    {
        add
        {
            if (this.OnPostDataUpdateScoped == null)
                this.parentService.PostDataUpdate += this.OnPostDataUpdateForward;

            this.OnPostDataUpdateScoped += value;
        }

        remove
        {
            this.OnPostDataUpdateScoped -= value;
            if (this.OnPostDataUpdateScoped == null)
                this.parentService.PostDataUpdate -= this.OnPostDataUpdateForward;
        }
    }

    private event INamePlateGui.PlateUpdateDelegate? OnNamePlateUpdateScoped;

    private event INamePlateGui.PlateUpdateDelegate? OnPostNamePlateUpdateScoped;

    private event INamePlateGui.PlateUpdateDelegate? OnDataUpdateScoped;

    private event INamePlateGui.PlateUpdateDelegate? OnPostDataUpdateScoped;

    /// <inheritdoc/>
    public void RequestRedraw()
    {
        this.parentService.RequestRedraw();
    }

    /// <inheritdoc/>
    public void DisposeService()
    {
        this.parentService.NamePlateUpdate -= this.OnNamePlateUpdateForward;
        this.OnNamePlateUpdateScoped = null;

        this.parentService.PostNamePlateUpdate -= this.OnPostNamePlateUpdateForward;
        this.OnPostNamePlateUpdateScoped = null;

        this.parentService.DataUpdate -= this.OnDataUpdateForward;
        this.OnDataUpdateScoped = null;

        this.parentService.PostDataUpdate -= this.OnPostDataUpdateForward;
        this.OnPostDataUpdateScoped = null;
    }

    private void OnNamePlateUpdateForward(
        INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        this.OnNamePlateUpdateScoped?.Invoke(context, handlers);
    }

    private void OnPostNamePlateUpdateForward(
        INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        this.OnPostNamePlateUpdateScoped?.Invoke(context, handlers);
    }

    private void OnDataUpdateForward(
        INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        this.OnDataUpdateScoped?.Invoke(context, handlers);
    }

    private void OnPostDataUpdateForward(
        INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        this.OnPostDataUpdateScoped?.Invoke(context, handlers);
    }
}
