
public class SimplifiedController
{
    public List<string> ModelStateErrors = new List<string>();

    public ViewAction View() => new ViewAction("something", null);

    public ViewAction View(OutputModel viewModel) => new ViewAction("something", viewModel);
}
