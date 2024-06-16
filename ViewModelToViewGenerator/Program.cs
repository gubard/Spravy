var directory = new DirectoryInfo("../../../../Spravy.Ui");
Console.WriteLine($"Directory: {directory}");

var viewModels = directory.GetFiles("*ViewModel.cs", SearchOption.AllDirectories)
   .Select(x => Path.GetFileNameWithoutExtension(x.Name))
   .Distinct()
   .ToArray();

var views = directory.GetFiles("*View.axaml", SearchOption.AllDirectories)
   .Select(x => Path.GetFileNameWithoutExtension(x.Name))
   .Distinct()
   .ToArray();

var commands = directory.GetFiles("*Commands.cs", SearchOption.AllDirectories)
   .Select(x => Path.GetFileNameWithoutExtension(x.Name))
   .Distinct()
   .ToArray();

foreach (var viewModel in viewModels)
{
    var view = viewModel.Replace("ViewModel", "View");

    if (views.Contains(view))
    {
        Console.WriteLine($"[Transient(typeof({viewModel}))]");
        Console.WriteLine($"[Transient(typeof({view}))]");
        var command = viewModel.Replace("ViewModel", "Commands");

        if (commands.Contains(command))
        {
            Console.WriteLine($"[Transient(typeof({command}))]");
        }
    }
}

foreach (var viewModel in viewModels)
{
    var view = viewModel.Replace("ViewModel", "View");

    if (views.Contains(view))
    {
        Console.WriteLine($$"""
            if (typeof({{viewModel}}) == viewModelType)
            {
                  return new (serviceFactory.CreateService<{{view}}>());
            }
            """);
    }
}