using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class ElementStyle_js
    {
        private HtmlElement Element; 
        public ElementStyle_js(HtmlElement Element)
        {
            this.Element = Element;
        }

        public String_js Display
        {
            get => new String_js { Name = Element.HowGet + ".style.display" };
            set => Display.Set(value);
        }

        public String_js AnimationName
        {
            get => new String_js { Name = Element.HowGet + ".style.animationName" };
            set => AnimationName.Set(value);
        }

        public String_js opacity
        {
            get => new String_js { Name = Element.HowGet + ".style.opacity" };
            set => opacity.Set(value);
        }

        public String_js transition
        {
            get => new String_js { Name = Element.HowGet + ".style.transition" };
            set => transition.Set(value);
        }

        public Runable_js OnAnimationEnd
        {
            get=> new Runable_js()
            {
                HowGet = Element.Attribute("onanimationend"),
                HowSet = (Value) => Element.Attribute("onanimationend").Set(Value)
            };
            set => OnAnimationEnd.Set(value);
        }
    }

    public class StyleValues_js
    {
        public Style.Dispaly_js Display
        {
            get => new Style.Dispaly_js();
        }
    }
}
