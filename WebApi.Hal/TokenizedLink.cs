#region

using System;
using System.Linq;

#endregion

namespace WebApi.Hal
{


    public class TokenizedLink : Link
    {
        const string HrefSlash = @"/";
        const string HrefQueryStringStart = @"?";
        const string HrefQueryStringAppend = @"&";
        readonly string hrefQueryString;
        readonly int hrefQueryStringStartPosition = -1;
        readonly string origHref;

        public TokenizedLink(string rel, string href)
            : base(rel, href)
        {
            origHref = href;
            hrefQueryStringStartPosition = PositionInHref(HrefQueryStringStart);
            hrefQueryString = hrefQueryStringStartPosition > -1
                                   ? origHref.Substring(hrefQueryStringStartPosition)
                                   : string.Empty;
        }

        public TokenizedLink CreateLink(Func<string, object>[] substitutions, bool removeIfNull = false)
        {
            foreach (var substitution in substitutions)
            {
                CreateLink(substitution, removeIfNull);
            }
            return this;
        }

        public TokenizedLink CreateLink(Func<string, object> substitution, bool removeIfNull = false)
        {
            UpdateHref(substitution, removeIfNull);

            return this;
        }

        public TokenizedLink TemplateWithoutQuerystring()
        {
            Href = origHref.Replace(hrefQueryString, string.Empty);
            return this;
        }

        void UpdateHref(Func<string, object> substitution, bool removeIfNull)
        {
            var subName = substitution.Method.GetParameters()[0].Name.Trim('_');
            var token = string.Format("{{{0}}}", subName);

            if (PositionInHref(token) <= -1) return;

            var val = substitution(null);
            var hasVal = val != null;

            if (hasVal)
            {
                Href = CreateUri(substitution).ToString();
                return;
            }

            if (!removeIfNull) return;

            if (!IsQuerystring(token))
                Href = Href.Replace(token + HrefSlash, string.Empty);
            else
            {
                //remove querystring pairs if remove is true
                if (string.IsNullOrWhiteSpace(hrefQueryString))
                    return;

                var queryVals =
                    hrefQueryString.Split(new[] {HrefQueryStringStart, HrefQueryStringAppend},
                                           StringSplitOptions.None).ToList();
                queryVals.RemoveAll(v => v.Contains(token));

                var newQueryVals = string.Join(HrefQueryStringAppend, queryVals);

                Href = Href.Replace(hrefQueryString, string.Empty);
                if (!string.IsNullOrWhiteSpace(newQueryVals))
                    Href = Href + HrefQueryStringStart + newQueryVals;
            }
        }


        bool IsQuerystring(string token)
        {
            if (hrefQueryStringStartPosition < 0) return false;
            return PositionInHref(token) > hrefQueryStringStartPosition;
        }

        int PositionInHref(string searchFor)
        {
            return PositionIn(searchFor, origHref);
        }

        static int PositionIn(string searchFor, string searchIn)
        {
            return searchIn.IndexOf(searchFor, StringComparison.Ordinal);
        }
    }

}