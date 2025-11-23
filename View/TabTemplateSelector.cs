using System.Windows;
using System.Windows.Controls;
using TestViewApp.Model;
using TestViewApp.ViewModel;

namespace TestViewApp.View
{
    public class TabTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is IListItem listItem) || !(container is FrameworkElement containerFrameworkElement))
            {
                return null;
            }

            switch (listItem)
            {
                case BuildDefinitionTestListViewModel p:
                    return FindDataTemplate(containerFrameworkElement, "BuildDefinitionTestListView");
                case TestStatisticsViewModel p:
                    return FindDataTemplate(containerFrameworkElement, "TestStatisticsListView");
                default:
                    throw new ArgumentException(nameof(item));
            }
        }

        private static DataTemplate FindDataTemplate(FrameworkElement containerFrameworkElement, string key)
        {
            return (DataTemplate)containerFrameworkElement.FindResource(key);
        }
    }
}
