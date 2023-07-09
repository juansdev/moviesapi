using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesApi.Helpers;

public class TypeBinder<T> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var propertyName = bindingContext.ModelName;
        var valuesProvider = bindingContext.ValueProvider.GetValue(propertyName);
        if (valuesProvider == ValueProviderResult.None) return Task.CompletedTask;

        try
        {
            var deserializeValue = JsonConvert.DeserializeObject<T>(valuesProvider.FirstValue);
            bindingContext.Result = ModelBindingResult.Success(deserializeValue);
        }
        catch
        {
            bindingContext.ModelState.TryAddModelError(propertyName, "Valor invalido para tipo List<int>");
        }

        return Task.CompletedTask;
    }
}