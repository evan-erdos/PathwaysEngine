/* Ben Scott * bescott@andrew.cmu.edu * 2015-11-13 * Inventory */

using System.Collections.Generic;
using adv=PathwaysEngine.Adventure;


/** `PathwaysEngine.Inventory` : **`namespace`**
 *
 * Deals with items, their abilities, and the inventory
 * window UI.
 **/
namespace PathwaysEngine.Inventory {


    /** `Keys` : **`enum`**
     *
     * Represents broad kinds of `LockKey` that can be used to
     * open things via `ILockable.Unlock()`.
     **/
    public enum Keys : int {
        Default  = 0,
        Breaker  = 1,
        Radial   = 2,
        Master   = 3,
        Skeleton = 4,
        Unique   = 5}


    public enum ItemStates : byte {
        Unused, Tarnished, Damaged, Broken }


    /** `IItem` : **`interface`**
     *
     * Interface to all `Item`s, deals with their storage
     **/
    public interface IItem : IStorable {


        /** `Held` : **`bool`**
         *
         * Is `this` held in inventory?
         **/
        bool Held {get;set;}


        /** `Mass` : **`real`**
         *
         * The physical mass of `this`.
         **/
        float Mass {get;set;}


        /** `Take()` : **`bool`**
         *
         * Called to inform `this` that it's been taken.
         **/
        bool Take();


        /** `Drop()` : **`bool`**
         *
         * Called to inform `this` that it's been dropped.
         **/
        bool Drop();
    }


    /** `IItemGroup<T>` : **`interface`**
     *
     * Manages groups of `Item`s, considers them a single
     * instance, (e.g., not `IEnumerable`)
     **/
    public interface IItemGroup<T> : IItem {


        /** `Count` : **`uint`**
         *
         * Represents the number of `Item`s that this group
         *
         **/
        uint Count {get;set;}


        /** `Group()` : **`function`**
         *
         * creates a group of `IItem`s
         **/
        void Group();


        /** `Split()` : **`IItemGroup<T>`**
         *
         * is the current `Item` held in inventory?
         **/
        IItemGroup<T> Split(uint n);
    }


    /** `IUsable` : **`interface`**
     *
     * Interface for items that can be used, and keeps
     * track of how many, if any, uses it has left.
     **/
    public interface IUsable : IItem {


        /** `Uses` : **`uint`**
         *
         * How many uses this `IItem` has left.
         **/
        uint Uses {get;set;}


        /** `Use()` : **`bool`**
         *
         * Use the `IItem`.
         **/
        bool Use();
    }


    /** `IGainful` : **`interface`**
     *
     * Interface for items that have monetary value, and
     * can be traded.
     **/
    public interface IGainful : IItem {


        /** `Cost` : **`int`**
         *
         * Price of an `IItem`.
         **/
        int Cost {get;set;}


        /** `Buy()` : **`bool`**
         *
         * Purchases an `IItem`.
         **/
        bool Buy();


        /** `Sell()` : **`bool`**
         *
         * Sells this `IItem`.
         **/
        bool Sell();
    }


    /** `IWearable` : **`interface`**
     *
     * Interface to anything that can be worn.
     **/
    public interface IWearable : IItem {


        /** `Worn` : **`bool`**
         *
         * Is this currently being worn by some `Actor`?
         **/
        bool Worn {get;set;}


        /** `Wear` : **`bool`**
         *
         * Equip this `IItem`.
         **/
        bool Wear();


        /** `Stow` : **`bool`**
         *
         * Put away this `IItem`.
         **/
        bool Stow();
    }


    /** `IWieldable` : **`interface`**
     *
     * Interface to anything that can be used to attack
     * someone, or alternatively, heal them, violently.
     **/
    public interface IWieldable : IWearable, IUsable {


        /** `Attack()` : **`bool`**
         *
         * Event to call when being used to attack.
         **/
        bool Attack();
    }


    /** `IItemSet` : **`interface`**
     *
     * This interface defines a collection of `Item`s, which
     * can be iterated over and queried for particular types of
     * items.
     **/
    public interface IItemSet : ICollection<Item>, IEnumerable<Item> {


        void Add<T>(ICollection<T> list) where T : Item;


        /** `GetItems<T>()` : **`<T>[]`**
         *
         * Gets all items in the `IItemSet` whose type is
         * either `<T>` or derives from `<T>`.
         **/
        List<T> GetItems<T>() where T : Item;


        /** `GetItem<T>()` : **`<T>`**
         *
         * Gets a single `Item` of type `<T>` from the set. If
         * there is no `Item` of the specified type, an `Item`
         * of a derived type may be returned.
         **/
        T GetItem<T>() where T : Item;
    }



    public static class Items {
        public static readonly IItemSet items;

        static Items() {
#if Index_Items
            var dict = new Dictionary<type,Item[]>() {
                {typeof(Item),Object.FindObjectsOfType<Item>() as Item[]}};
            foreach (var elem in dict[typeof(Item)]) {
                if (elem.GetType()!=typeof(Item))
                    dict[elem.GetType()] = GetItems(
                        elem.GetType(),dict);
            } items=dict; // readonlys must be constructed here
#endif
        }
#if TODO
        public static Item GetItem<T>() where T : Item {
            return GetItem<T>(items); }

        public static Item GetItem<T>(IItemSet dict) where T : Item {
            List<Item> temp;
            if (dict.TryGetValue(typeof(T),out temp)
            && temp!=null && temp.Count>0 && temp[0])
                return temp[0];
            else return default (Item);
        }

        public static List<Item> GetItems<T>() where T : Item {
            return GetItems(typeof(T),items); }

        public static List<Item> GetItems<T>(IItemSet dict) {
            List<Item> temp = new List<Item>();
            if (typeof(T)==typeof(Item)
            && dict.ContainsKey(typeof(T)))
                return dict[typeof(Item)];
            if (typeof(T).IsSubclassOf(typeof(Item))
            && dict.ContainsKey(typeof(T)))
                return dict[typeof (T)];
            foreach (var elem in dict[typeof(Item)])
                if (elem.GetType()==typeof (T)) temp.Add(elem);
            return temp;
        }

        static List<Item> GetItems(type T, IItemSet dict) {
            List<Item> temp = new List<Item>();
            if (T==typeof(Item) && dict.ContainsKey(T))
                return dict[typeof(Item)];
            if (T.IsSubclassOf(typeof(Item))
            && dict.ContainsKey(T)) return dict[T];
            foreach (var elem in dict[typeof(Item)])
                if (elem.GetType()==T) temp.Add(elem);
            return temp;
        }

        static List<Item> GetItems(type T) {
            return GetItems(T,items); }
#endif
    }
}

