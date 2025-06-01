using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HWT.Domain.Entities
{
    public class CommodityFuture : INotifyPropertyChanged
    {
        private int    _id;
        private int?   _idParent;
        private string _name      = string.Empty;
        private string _code      = string.Empty;
        private string _kind      = string.Empty;
        private double _weightScu;
        private float  _priceBuy;
        private float  _priceSell;
        private int    _isAvailable;
        private int    _isAvailableLive;
        private int    _isVisible;
        private int    _isExtractable;
        private int    _isMineral;
        private int    _isRaw;
        private int    _isRefined;
        private int    _isRefinable;
        private int    _isHarvestable;
        private int    _isBuyable;
        private int    _isSellable;
        private int    _isTemporary;
        private int    _isIllegal;
        private int    _isVolatileQt;
        private int    _isVolatileTime;
        private int    _isInert;
        private int    _isExplosive;
        private int    _isFuel;
        private int    _isBuggy;
        private string _wiki      = string.Empty;
        private long   _dateAdded;
        private long   _dateModified;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public int? IdParent
        {
            get => _idParent;
            set => SetField(ref _idParent, value);
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Code
        {
            get => _code;
            set => SetField(ref _code, value);
        }

        public string Kind
        {
            get => _kind;
            set => SetField(ref _kind, value);
        }

        public double WeightScu
        {
            get => _weightScu;
            set => SetField(ref _weightScu, value);
        }

        public float PriceBuy
        {
            get => _priceBuy;
            set => SetField(ref _priceBuy, value);
        }

        public float PriceSell
        {
            get => _priceSell;
            set => SetField(ref _priceSell, value);
        }

        /// <summary>
        /// “is_available” flag from API (1 = available, 0 = not)
        /// </summary>
        public int IsAvailable
        {
            get => _isAvailable;
            set => SetField(ref _isAvailable, value);
        }

        /// <summary>
        /// “is_available_live” from API (live-status in Star Citizen)
        /// </summary>
        public int IsAvailableLive
        {
            get => _isAvailableLive;
            set => SetField(ref _isAvailableLive, value);
        }

        public int IsVisible
        {
            get => _isVisible;
            set => SetField(ref _isVisible, value);
        }

        public int IsExtractable
        {
            get => _isExtractable;
            set => SetField(ref _isExtractable, value);
        }

        public int IsMineral
        {
            get => _isMineral;
            set => SetField(ref _isMineral, value);
        }

        public int IsRaw
        {
            get => _isRaw;
            set => SetField(ref _isRaw, value);
        }

        public int IsRefined
        {
            get => _isRefined;
            set => SetField(ref _isRefined, value);
        }

        public int IsRefinable
        {
            get => _isRefinable;
            set => SetField(ref _isRefinable, value);
        }

        public int IsHarvestable
        {
            get => _isHarvestable;
            set => SetField(ref _isHarvestable, value);
        }

        public int IsBuyable
        {
            get => _isBuyable;
            set => SetField(ref _isBuyable, value);
        }

        public int IsSellable
        {
            get => _isSellable;
            set => SetField(ref _isSellable, value);
        }

        public int IsTemporary
        {
            get => _isTemporary;
            set => SetField(ref _isTemporary, value);
        }

        public int IsIllegal
        {
            get => _isIllegal;
            set => SetField(ref _isIllegal, value);
        }

        public int IsVolatileQt
        {
            get => _isVolatileQt;
            set => SetField(ref _isVolatileQt, value);
        }

        public int IsVolatileTime
        {
            get => _isVolatileTime;
            set => SetField(ref _isVolatileTime, value);
        }

        public int IsInert
        {
            get => _isInert;
            set => SetField(ref _isInert, value);
        }

        public int IsExplosive
        {
            get => _isExplosive;
            set => SetField(ref _isExplosive, value);
        }

        public int IsFuel
        {
            get => _isFuel;
            set => SetField(ref _isFuel, value);
        }

        public int IsBuggy
        {
            get => _isBuggy;
            set => SetField(ref _isBuggy, value);
        }

        public string Wiki
        {
            get => _wiki;
            set => SetField(ref _wiki, value);
        }

        public long DateAdded
        {
            get => _dateAdded;
            set => SetField(ref _dateAdded, value);
        }

        public long DateModified
        {
            get => _dateModified;
            set => SetField(ref _dateModified, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Helper to set the field and raise PropertyChanged if value changed.
        /// </summary>
        protected bool SetField<TField>(ref TField field, TField value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value!;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
