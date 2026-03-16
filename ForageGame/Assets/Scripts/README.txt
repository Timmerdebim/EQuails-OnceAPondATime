Scripts Documentation

------------ File Structure ------------

Core    :   Core systems; (Almost) No dependencies on eachother. Absolutely no dependencies on feature modules or game.
            TLDR; stable, generic, & reusable 
            (Hint: should never really feature any controllers or animators unless abstract!)
Features:   Feature modules; Only dependent on core systems.
            TLDR; composes from whichever Core systems it needs.
Game    :   Game scripts; Dependent on all core systems and feature modules.
            TLDR; the actual scenes and glue that wire everything together.

Example:
    The Interaction System and Gadget System belong in Core.
    The Switch uses both of these and hence belongs in Features.

Within Core / Features / Game there will be individual folders corresponding to namespaces.
For core, this should (almost) always be a XXXSystem namespace. For example; Core > GadgetSystem.
For features this will either be the feature, or a collection of features. For example; Features > Gadgets > SwitchGadget.

Within a module (both core and feature) you will find the following possible folders (not all are required)


Editor      : Compiles into a Unity Editor-only assembly (Not available in builds) (Unity reserved folder name reference)
Resources   : The game's runtime assembly? (?) (Unity reserved folder name reference)
Runtime     : The game's runtime assembly (Available in builds)
Tests       : A test-only assembly (Not available in builds)

------------ Class Prefix ------------

I
    Interfaces.
    public interface I[---]

------------ Class Suffix ------------

Service
    Stateless cross-cutting utility
    A stateless or near-stateless utility used across many systems. Often injected as a dependency.
    public class [---]Service : MonoBehaviour, I[---]Service

System
    Owns a domain.
    Owns and coordinates a domain. Usually a singleton or service. Nothing outside it should replicate its core logic.
    public class [---]System : ScriptableObject
    public class [---]System : MonoBehaviour # Instance
    public static class [---]System

Manager 
    Avoid this one. It means nothing specific; tends to become a dumping ground. 
    If you find yourself writing SwitchManager, ask what it actually does and rename it accordingly. System covers the coordination role more honestly.
    public class [---]Manager : MonoBehaviour # Instance

Controller
    Drives a specific object.
    Drives behaviour on a specific object in the scene.
    Takes input and translates it into actions on that object.
    public class [---]Controller # : optional abstract/interface inheritance

Animator
    Visual/audio feedback only.
    Purely handles visual/audio feedback. Should contain zero game logic — if an Animator is making gameplay decisions, that code belongs in a Controller.
    Requires a Controller.
    public class [---]Animator # : optional abstract inheritance

Handler
    Reacts to one event. 
    Responds to a specific event or message. Narrower than a Controller — it doesn't drive, it reacts.
    public class [---]Handler

Registry
    Dynamic collection built at runtime.
    Stores and provides lookup for a collection of things at runtime.
    public static class [---]Registry
    public class [---]Registry : ScriptableObject

Database — 
    Constant collection built before runtime.
    Stores and provides lookup for a collection of things at runtime.
    public static class [---]Database
    public class [---]Database : ScriptableObject

Data
    Plain data container
    A plain container, no behaviour. Usually a ScriptableObject or serializable struct.
    public struct [---]Data # Should be [Serializable]
    public class [---]Data : ScriptableObject

Config
    Designer-facing tuning.
    Similar to Data but specifically for tunable settings, often designer-facing. The distinction from Data is intent: Config is for tweaking behaviour, Data is for defining an entity.
    public class [---]Config : ScriptableObject

FSM
    public class [---]FSM : FSM

State
    One node in a state machine.
    Represents one state in a state machine. Implies the class is never used standalone.
    public class [---]State : State (? or IState)

Factory
    Creates instances.
    Creates instances of something. Hides instantiation complexity from callers.
    public static class [---]Factory

Spawner
    Scene-level instantiation.
    A scene-level object responsible for creating things at runtime.
    public class [---]Spawner : MonoBehaviour

Trigger — A thin scene object whose only job is to detect something and notify others. Should have no logic of its own.
    Detects and notifies, no logic.
    public class [---]Trigger : MonoBehaviour

View
    Displays state, no logic
    Displays state, owns no logic. Common if you use MVC/MVP patterns.

Inspector
    Requires [CustomEditor(typeof([---]), true)]
    public class [---]Inspector : UnityEditor.Editor

------------ Method Prefix ------------

T Get[---]()        :   returns a type of [---].

void Set[---](T t)  :   sets a type of [---].
void Add[---](T t)  :   adds a type of T to [---].
void Remove[---](T t):  removes a type of T to [---].

bool Try[---]       :   tries to do the method; return successes.

------------ Fields, Properties, Variables, & Parameters------------

PascalCase
    public
    record constructor parameter

_camelCase
    private f
    internal

s_camelCase
    private static
    internal static

t_camelCase
    thread static

camelCase
    local variable
    method parameter
    class/struct parameter