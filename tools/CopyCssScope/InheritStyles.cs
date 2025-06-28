using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace CopyCssScope;

public class InheritStyles : Task
{
    private const string InheritMetadata = "Inherit";
    private const string CssScopeMetadata = "CssScope";
    
    [Required]
    public ITaskItem[] Items { get; set; }
    
    [Required]
    public ITaskItem[] AllComponents { get; set; }
    
    [Required]
    public ITaskItem[] AllStylesheets { get; set; }
    
    [Output]
    public ITaskItem[] ModifiedComponents { get; set; }
    
    [Output]
    public ITaskItem[] ModifiedStylesheets { get; set; }

    public override bool Execute()
    {
        var modifiedComponents = new List<ITaskItem>();
        var modifiedStylesheets = new List<ITaskItem>();
        
        foreach (var item in Items)
        {
            var inheritedComponent = AllComponents.SingleOrDefault(s => s.ItemSpec == item.GetMetadata(InheritMetadata));
            if (inheritedComponent is null)
            {
                Log.LogWarning("Component {0} cannot inherit styles from {1}: Component not found.", item.ItemSpec, item.GetMetadata(InheritMetadata));
                continue;
            }

            var definedComponent = AllComponents.SingleOrDefault(s => s.ItemSpec == item.ItemSpec);
            if (definedComponent is not null)
            {
                definedComponent.SetMetadata(CssScopeMetadata, inheritedComponent.GetMetadata(CssScopeMetadata));
                modifiedComponents.Add(definedComponent);
            }

            var definedStyle = AllStylesheets.SingleOrDefault(s => s.ItemSpec == item.ItemSpec + ".css");
            if (definedStyle is not null)
            {
                definedStyle.SetMetadata(CssScopeMetadata, inheritedComponent.GetMetadata(CssScopeMetadata));
                modifiedStylesheets.Add(definedStyle);
            }
        }
        
        ModifiedComponents = modifiedComponents.ToArray();
        ModifiedStylesheets = modifiedStylesheets.ToArray();

        return true;
    }
}
