﻿using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// Represents options for making JSON diff.
    /// </summary>
    public class JsonDiffOptions
    {
        internal static readonly JsonDiffOptions Default = new();

        /// <summary>
        /// Specifies whether to suppress detect array move. Default value is <c>false</c>.
        /// </summary>
        public bool SuppressDetectArrayMove { get; set; }

        /// <summary>
        /// Gets or sets the function to match array items.
        /// </summary>
        public ArrayItemMatch? ArrayItemMatcher { get; set; }

        /// <summary>
        /// Gets or sets the function to find key of a <see cref="JsonObject"/>
        /// or <see cref="JsonArray"/>. This is used when matching array items by
        /// their keys. If this function returns <c>null</c>, the items being
        /// compared are treated as "not keyed". When comparing two "not keyed"
        /// objects, their contents are compared. This function is only used when
        /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>.
        /// </summary>
        public Func<JsonNode?, int, object?>? ArrayObjectItemKeyFinder { get; set; }

        /// <summary>
        /// Gets or sets whether two instances of JSON object types (object and array)
        /// are considered equal if their position is the same in their parent
        /// arrays regardless of their contents. This property is only used when
        /// <see cref="ArrayItemMatcher"/> is set to <c>null</c>. By settings this
        /// property to <c>true</c>, a diff could be returned faster but larger in
        /// size. Default value is <c>false</c>.
        /// </summary>
        public bool ArrayObjectItemMatchByPosition { get; set; }

        /// <summary>
        /// Gets or sets whether to prefer <see cref="ArrayObjectItemKeyFinder"/> and
        /// <see cref="ArrayObjectItemMatchByPosition"/> than using deep value comparison
        /// to match array object items. By settings this property to <c>true</c>,
        /// a diff could be returned faster but larger in size. Default value is <c>false</c>.
        /// </summary>
        public bool PreferFuzzyArrayItemMatch { get; set; }

        /// <summary>
        /// Gets or sets the minimum length for diffing texts using <see cref="TextDiffProvider"/>
        /// or default text diffing algorithm, aka Google's diff-match-patch algorithm. When text
        /// diffing algorithm is not used, text diffing is fallback to value replacement. If this
        /// property is set to <c>0</c>, diffing algorithm is disabled. Default value is <c>0</c>.
        /// </summary>
        public int TextDiffMinLength { get; set; }

        /// <summary>
        /// Gets or sets the function to diff long texts.
        /// </summary>
        public Func<string, string, string?>? TextDiffProvider { get; set; }
    }
}
