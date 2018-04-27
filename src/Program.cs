using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.Actions.ActionManager;
using JetBrains.Application.UI.ActionsRevised.Handlers;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.Action;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.Text;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.StructuralNavigation;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.Sessions;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;

namespace Ulex.TabCompletion
{
  [Action("InvokeCompletion")]
  [VsOverrideAction("({1496a755-94de-11d0-8c3f-00c04fc2aae2}:4)")] // Edit.InsertTab
  public class InvokeCompletionHandler : OverridingActionHandler
  {
    public InvokeCompletionHandler()
      : base(TextControlActions.TAB_ACTION_ID)
    {
    } 
    
    public override bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
      if (textControl == null)
        return nextUpdate();

      var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
      if (solution == null)
        return nextUpdate();

      if (ShouldFallback(textControl, context))
        return nextUpdate();

      return true;
    }

    private bool ShouldFallback(ITextControl textControl, IDataContext context)
    {
      var structuralNavigationManager = context.TryGetComponent<StructuralNavigationManager>();
      if (structuralNavigationManager == null || structuralNavigationManager.IsDisabledByIntellisense(textControl))
        return true;

      var psiSourceFile = context.GetData(PsiDataConstants.SOURCE_FILE);
      if (psiSourceFile == null)
        return true;

      return false;
    }

    public override void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
      var solution = context.GetData(ProjectModelDataConstants.SOLUTION);

      if (textControl == null || solution == null || ShouldFallback(textControl, context) ||
          solution.GetComponent<CodeCompletionSessionManager>().Session != null)
      {
        nextExecute();
        return;
      }

      var actionManager = solution.GetComponent<IActionManager>();
      var actionDef = actionManager.Defs.GetActionDef<CompleteCodeBasicAction>();

      actionDef.EvaluateAndExecute(actionManager, context);
    }
  }
}
