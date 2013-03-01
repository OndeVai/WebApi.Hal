#region

using System;
using System.Linq;

#endregion

namespace WebApi.Hal
{
    public class TokenizedLink : Link
    {
        private const string HrefSlash = @"/";
        private const string HrefQueryStringStart = @"?";
        private const string HrefQueryStringAppend = @"&";

        private readonly string _hrefQueryString;
        private readonly int _hrefQueryStringStartPosition = -1;
        private readonly string _origHref;

        public TokenizedLink(string rel, string href)
            : base(rel, href)
        {
            _origHref = href;
            _hrefQueryStringStartPosition = PositionInHref(HrefQueryStringStart, this);
            _hrefQueryString = _hrefQueryStringStartPosition > -1
                                   ? _origHref.Substring(_hrefQueryStringStartPosition)
                                   : string.Empty;
        }


        public TokenizedLink CreateLink(Func<string, object> substitution, bool removeIfNull = false)
        {
            var newLink = new TokenizedLink(Rel, Href);
            UpdateHref(newLink, substitution, removeIfNull);

            return newLink;
        }

        public TokenizedLink TemplateWithoutQuerystring()
        {
            var newLink = new TokenizedLink(Rel, _origHref.Replace(_hrefQueryString, string.Empty));
            return newLink;
        }

        private static void UpdateHref(TokenizedLink link, Func<string, object> substitution, bool removeIfNull)
        {
            var subName = substitution.Method.GetParameters()[0].Name.Trim('_');
            var token = string.Format("{{{0}}}", subName);

            if (PositionInHref(token, link) <= -1) return;

            var val = substitution(null);
            var hasVal = val != null;

            if (hasVal)
            {
                link.Href = link.CreateUri(substitution).ToString();
                return;
            }

            if (!removeIfNull) return;

            if (!IsQuerystring(token, link))
                link.Href = link.Href.Replace(token + HrefSlash, string.Empty);
            else
            {
                //remove querystring pairs if remove is true
                if (string.IsNullOrWhiteSpace(link._hrefQueryString))
                    return;

                var queryVals =
                    link._hrefQueryString.Split(new[] {HrefQueryStringStart, HrefQueryStringAppend},
                                                StringSplitOptions.RemoveEmptyEntries).ToList();
                queryVals.RemoveAll(v => v.Contains(token));

                var newQueryVals = string.Join(HrefQueryStringAppend, queryVals);

                link.Href = link.Href.Replace(link._hrefQueryString, string.Empty);
                if (!string.IsNullOrWhiteSpace(newQueryVals))
                    link.Href = link.Href + HrefQueryStringStart + newQueryVals;
            }
        }


        private static bool IsQuerystring(string token, TokenizedLink link)
        {
            var hrefQueryStringStartPosition = link._hrefQueryStringStartPosition;
            if (hrefQueryStringStartPosition < 0) return false;
            return PositionInHref(token, link) > hrefQueryStringStartPosition;
        }

        private static int PositionInHref(string searchFor, TokenizedLink link)
        {
            var origHref = link._origHref;
            return PositionIn(searchFor, origHref);
        }

        private static int PositionIn(string searchFor, string searchIn)
        {
            return searchIn.IndexOf(searchFor, StringComparison.Ordinal);
        }
    }
}