namespace FunctionalCore
{
    /// <summary>
    /// Represents a value-less type used in functional programming.
    /// 関数型プログラミングで使用される、値を持たない型を表す。
    /// </summary>
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <summary>
        /// Singleton instance of Unit.
        /// Unit のシングルトンインスタンス。
        /// </summary>
        public static readonly Unit Value = new();

        /// <summary>
        /// Always returns true because all Unit values are equal.
        /// Unit は常に等しいため、常に true を返す。
        /// </summary>
        public bool Equals(Unit other) => true;

        /// <summary>
        /// Determines whether this instance equals another object.
        /// このインスタンスが別のオブジェクトと等しいかどうかを判定する。
        /// </summary>
        public override bool Equals(object? obj) => obj is Unit;

        /// <summary>
        /// Returns a constant hash code because all Unit values are equal.
        /// Unit は常に等しいため、常に同じハッシュコードを返す。
        /// </summary>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Returns a string representation of Unit.
        /// Unit の文字列表現を返す。
        /// </summary>
        public override string ToString() => "()";

        /// <summary>
        /// Equality operator.
        /// 等値演算子。
        /// </summary>
        public static bool operator ==(Unit left, Unit right) => true;

        /// <summary>
        /// Inequality operator.
        /// 不等値演算子。
        /// </summary>
        public static bool operator !=(Unit left, Unit right) => false;
    }
}
