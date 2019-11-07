using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Contract
{
    [Serializable]
    public class LabelImageAdded
    {
        public string ItemName { get; set; }
        public string[] Ingredients { get; set; }
        
    }
}
