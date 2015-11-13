/* Ben Scott * bescott@andrew.cmu.edu * 2015-11-13 * Inventory */

using intf=PathwaysEngine.Adventure;

/** `PathwaysEngine.Inventory` : **`namespace`**
 *
 * Deals with items, their abilities, and the inventory
 * window UI.
 **/
namespace PathwaysEngine.Inventory {
	public enum ItemStates : byte { Unused, Tarnished, Damaged, Broken }

	/** `IItem` : **`interface`**
	 *
	 * Interface to all `Item`s, deals with their storage
	 **/
	public interface IItem : intf::IStorable {

		/** `held` : **`bool`**
		 *
		 * is the current `Item` held in inventory?
		 **/
		bool Held { get; set; }

		/** `Take()` : **`void`**
		 *
		 * Called to inform the `IItem` that it's been
		 * taken. Sets `Rigidbody.isKinematic`, etc.
		 **/
		void Take();

		/** `Drop()` : **`void`**
		 *
		 * Called to inform the `IItem` that it's been
		 * dropped. Sets `Rigidbody.isKinematic`, etc.
		 **/
		void Drop();
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
		uint Count { get; set; }

		/** `Group()` : **`void`**
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
		uint Uses { get; set; }

		/** `Use()` : **`void`**
		 *
		 * Use the `IItem`.
		 **/
		void Use();
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
		int Cost { get; set; }

		/** `Buy()` : **`void`**
		 *
		 * Purchases an `IItem`.
		 **/
		void Buy();

		/** `Sell()` : **`void`**
		 *
		 * Sells this `IItem`.
		 **/
		void Sell();
	}

	/** `IReadable` : **`interface`**
	 *
	 * Interface to anything that can be read.
	 **/
	public interface IReadable : IUsable {

		/** `Read()` : **`void`**
		 *
		 * Function to call when reading something.
		 **/
		void Read();
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
		bool Worn { get; set; }

		/** `Wear` : **`void`**
		 *
		 * Equip this `IItem`.
		 **/
		void Wear();

		/** `Stow` : **`void`**
		 *
		 * Put away this `IItem`.
		 **/
		void Stow();
	}

	/** `IWieldable` : **`interface`**
	 *
	 * Interface to anything that can be used to attack
	 * someone, or alternatively, heal them, violently.
	 **/
	public interface IWieldable : IWearable, IUsable {

		/** `Attack()` : **`void`**
		 *
		 * Event to call when being used to attack.
		 **/
		void Attack();
	}

	public static class Items {
		public static readonly IItemSet items;

		static Items() {
#if Index_Items
			var dict = new Dictionary<type,Item[]>() {
				{typeof(Item),Object.FindObjectsOfType<Item>() as Item[]}};
			foreach (var elem in dict[typeof(Item)]) {
				if (elem.GetType()!=typeof(Item))
					dict[elem.GetType()] = GetItemsOfType(
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

		public static List<Item> GetItemsOfType<T>() where T : Item {
			return GetItemsOfType(typeof(T),items); }

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

		static List<Item> GetItemsOfType(type T, IItemSet dict) {
			List<Item> temp = new List<Item>();
			if (T==typeof(Item) && dict.ContainsKey(T))
				return dict[typeof(Item)];
			if (T.IsSubclassOf(typeof(Item))
			&& dict.ContainsKey(T)) return dict[T];
			foreach (var elem in dict[typeof(Item)])
				if (elem.GetType()==T) temp.Add(elem);
			return temp;
		}

		static List<Item> GetItemsOfType(type T) {
			return GetItemsOfType(T,items); }
#endif
	}
}
