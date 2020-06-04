namespace WPF_Project_Countries.Services
{
    #region Usings

    using System.Windows;

    #endregion

    public class DialogService
    {
        #region Methods (alphabetical order)

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        #endregion
    }
}