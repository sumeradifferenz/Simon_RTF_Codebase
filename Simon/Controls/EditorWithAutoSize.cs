using Xamarin.Forms;

namespace Simon.Controls
{
    public class EditorWithAutoSize : Editor
    {
        public EditorWithAutoSize()
        {
            TextChanged += (sender, e) =>
            {
                InvalidateMeasure();
            };
        }
    }
}
