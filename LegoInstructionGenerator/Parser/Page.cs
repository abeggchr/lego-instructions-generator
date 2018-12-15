using System.Collections.Generic;

namespace LegoInstructionGenerator.Parser
{
    internal class Page
    {
        internal Page(int index)
        {
            Index = index;
            Text = new List<string>();
        }

        public string MainImage { get; set; }

        public string PartsImage { get; set; }

        public int PartsImageCropFactor { get; set; }

        /// <summary>
        ///     1-based index of the page.
        /// </summary>
        public int Index { get; }

        public List<string> Text { get; set; }

        public bool HasPartsImage => !string.IsNullOrEmpty(PartsImage);

        public bool HasText => Text.Count > 0;
    }
}