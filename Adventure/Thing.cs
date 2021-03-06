/* Ben Scott * bescott@andrew.cmu.edu * 2015-08-22 * Thing */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventArgs=System.EventArgs;
//using static PathwaysEngine.Literature.Terminal;
using lit=PathwaysEngine.Literature;


namespace PathwaysEngine.Adventure {


    /** `Thing` : **`MonoBehaviour`**
     *
     * An extremely important class, `Thing` is the base class
     * for anything that has any interface with the `Adventure`
     * namespace, the `Parser`, or the `Statistics`namespace,
     * or the `Terminal` or any deriving/text-based interfaces.
     **/
    public class Thing : MonoBehaviour, IThing {
        bool waitView;
        protected Rigidbody _rigidbody;
        protected Collider _collider;

        public virtual bool Seen {get;set;}

        public virtual float Radius => 5f;

        public string Name => gameObject.name;

        public virtual string Template => $@"
### {Name} ###
{description.init}{description.raw}
{description.Help}";

        public virtual Vector3 Position =>
            transform.position;

        public virtual lit::Description description {get;set;}

        public event lit::Parse ViewEvent {
            add { viewEvent += value; }
            remove { viewEvent -= value; }
        } event lit::Parse viewEvent;


        /** `AddListener()` : **`function`**
         *
         * Every instantiated `Thing` calls if it detects the
         * `Player` is nearby via `OnTriggerEnter()`. It then
         * subscribes itself to or unsubscribes itself from the
         * global/static `Thing`, acts as an event handler to
         * the instances. Subscribers could have any number of
         * functions called iff the `Player` is nearby and the
         * `Player` issues an appropriate command.
         **/
        public virtual void AddListener(Thing thing) =>
            ViewEvent += thing.View;


        /** `RemoveListener()` : **`function`**
         *
         * Corollary to the `AddListener()` function.
         **/
        public virtual void RemoveListener(Thing thing) =>
            ViewEvent -= thing.View;


        public virtual bool View(
                        Person sender,
                        EventArgs e,
                        lit::Command c,
                        string input) => View();

        public virtual bool View() {
            lit::Terminal.Log(this);
            Seen = true;
            description.Seen = true;
            return true;
        }

        public virtual bool Find() => false;


        public virtual bool Fits(string s) =>
            description.Nouns.IsMatch(s);


        IEnumerator Viewing() {
            waitView = true;
            View();
            yield return new WaitForSeconds(1f);
            waitView = false;
        }


        public virtual void Awake() {
            gameObject.layer = LayerMask.NameToLayer("Thing");
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody!=null) {
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }
            Deserialize();
            AddListener(this);
        }

        public virtual void Start() { }


        public virtual IEnumerator OnMouseOver() {
            while (Player.IsNear(this)) {
                Pathways.CursorGraphic = Cursors.Pick;
                if (Input.GetButtonUp("Fire1") && !waitView)
                    yield return StartCoroutine(Viewing());
                else yield return new WaitForSeconds(0.1f);
            } OnMouseExit();
        }

        public virtual void OnMouseExit() {
            Pathways.CursorGraphic = Cursors.None;
            StopAllCoroutines();
        }

        public virtual void Deserialize() =>
            Pathways.Deserialize<Thing,Thing_yml>(this);


        public override string ToString() => Name;

        public static bool operator !(Thing o) => (o==null);
    }
}
