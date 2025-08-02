using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Swallow.Validation;

namespace DynDNS.Web.Website.Validation;

public sealed class SwallowValidationProvider : ComponentBase, IDisposable
{
    private EditContext? attachedEditContext;
    private ValidationMessageStore? messageStore;

    [CascadingParameter]
    public required EditContext EditContext { get; init; }

    protected override void OnInitialized()
    {
        attachedEditContext = EditContext;
        messageStore = new ValidationMessageStore(attachedEditContext);

        attachedEditContext.OnValidationRequested += Validate;
        attachedEditContext.OnFieldChanged += FieldChange;
    }

    private void FieldChange(object? sender, FieldChangedEventArgs e)
    {
        if (attachedEditContext?.Model is IValidatable validatable)
        {
            var identifier = e.FieldIdentifier;
            messageStore?.Clear(identifier);

            var result = validatable.Validate();
            foreach (var message in result.Errors.Where(msg => msg.PropertyName == identifier.FieldName))
            {
                messageStore?.Add(identifier, message.Message);
            }

            attachedEditContext.NotifyValidationStateChanged();
        }
    }

    private void Validate(object? sender, ValidationRequestedEventArgs e)
    {
        if (attachedEditContext?.Model is IValidatable validatable)
        {
            messageStore?.Clear();

            var result = validatable.Validate();
            foreach (var message in result.Errors)
            {
                var identifier = new FieldIdentifier(validatable, message.PropertyName);
                messageStore?.Add(identifier, message.Message);
            }

            attachedEditContext.NotifyValidationStateChanged();
        }
    }

    protected override void OnParametersSet()
    {
        if (EditContext != attachedEditContext)
        {
            throw new InvalidOperationException($"{nameof(SwallowValidationProvider)} does not support changing the {nameof(EditContext)} dynamically.");
        }
    }

    public void Dispose()
    {
        if (attachedEditContext != null)
        {
            attachedEditContext.OnValidationRequested -= Validate;
            attachedEditContext.OnFieldChanged -= FieldChange;
        }
    }
}
