# IAV-Robot

# IAV - Base para el proyecto final
## Autor
- Pablo Sánchez Martín: pablosm196 - https://github.com/pablosm196

## Propuesta
Este es el proyecto final de la asignatura Inteligencia Artificial para Videojuegos.

Esta práctica se basa en el trabajo final de la asignatura Entornos Interactivos y Realidad Virtual. El trabajo consiste en un juego para las Meta Quest con dos escenarios: uno de realidad mixta y otro escenario creado a partir de los assets de https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529; en los que van apareciendo zombies cada cierto tiempo que van hacia el jugador, y en cuanto colisionan con el jugador pierde la partida.

El objetivo de la práctica es realizar un comportamiento inteligente para los zombies, en el que se les dotará de vista, oído y olfato. Los zombies estarán deambulando de forma aleatoria hasta que vean al jugador, escuchen un disparo o huelan el rastro del jugador.

Este proyecto se centrará en el escenario creado ya que los espacios en realidad mixta son demasiado pequeños para que se haga notable el comportamiento de los zombies.

## Punto de partida

Se parte de un proyecto base de **Unity 2022.3.18f1** creado para la asignatura Entornos Interactivos y Realidad Virtual por [Pablo Sánchez Martín](https://github.com/pablosm196) y [Javier Muñoz García](https://github.com/javimuno); y disponible en este repositorio: [Proyecto VR](https://github.com/pablosm196/ProyectoVR).

Para la funcionalidad en realidad virtual y mixta se ha usado el paquete de [Meta XR All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657).
Aquí se puede ver la [documentación del SDK](https://developer.oculus.com/documentation/unity/unity-import-samples/).

Con lo proporcionado como base se deberá plantear una IA que permita a los zombies moverse por el entorno y encontrar al jugador con su percepción. El comportamiento de los zombies será creado a partir de una máquina de estados finita (FSM) jerárquica y completamente dirigida por datos.

A continuación, se explicará la funcionalidad y los métodos de cada clase base. 

### Core: OVRCameraRig
Es la representación del jugador en en entorno virtual
```
 OVRCameraRig: Gestiona de las cámaras de las Meta Quest.
 OVRManager: Manager del SDK de Meta
 CharacterMovement: Componente creado que permite el movimiento y la rotación con los joysticks de los mandos de las Meta Quest
 PlayerHealth: Componente creado para gestionar las colisiones con los zombies
 Gun: Componente que contiene el RightHandAnchor (OVRCameraRig->TrackingSpace->RightHandAnchor, contiene la funcionalidad de la mano derecha) que permite disparar con el gatillo primario de la mano derecha.
```
### Core: Zombies
```
 EnemyHealth: Gestiona la salud de los enemigos.
 NavMeshAgent: Clase que perite mover al enemigo por el entorno navegable.
 FollowPlayer: Clase que permite al zombie seguir al jugador por el entorno.
 NavigateRandomPoint: Clase que permite al zombie elegir un punto aleatorio del entorno para ir hacia él.
```
### Core: LevelObjects
```
 Bullet: Comprueba si la bala colisiona con un enemigo. Si es así, se reduce su salud según el valor de daño del proyectil.
 Piedra: Objeto interactuable que si colisiona con un enemigo le mata.
 Martillo: Objeto interactuable que si colisiona con un enemigo le empuja y le baja vida.
```
### Managers
```
 GameManager:  Clase encargada de gestionar la aparición de los zombies.
 UIManager: Clase encargada de gestionar el conteo de puntos y del tiempo total.
```

### UI
```
 LookAt: Clase encargada de hacer que el canvas mire siempre al jugador.
 OVR Raycaster: Clase encargada de permitir la interacción a distancia con los botones de la interfaz del menú principal
```

## Diseño de la solución
### Mecánicas de juego
Las mecánicas de juego que se han implementado son las siguientes:

**Jugador:**
- Movimiento simple mediante el **joystick izquierdo**
- Rotación contínua mediante el **joystick derecho**
- Disparo mediante el **gatillo primario derecho**
- Interactuar con ciertos objetos mediante el **gatillo secundario izquierdo**

**Zombies/Enemigos:**
- Merodeo aleatorio por el escenario
- Capacidad de detectar al enemigo dentro de un cono de visión.
- Al detectar al personaje comienza a perseguirle.
    * En caso de perder de vista al jugador, volverá al merodeo aleatorio.
- Capacidad de escuchar los disparos del jugador.
    * Al escuchar un disparo, irá a la posición de la fuente del sonido.
- Capacidad de oler al jugador.
    * Los zombies podrán oler el rastro de olor que va dejando un jugador.

## Diagrama de comportamientos

Todas los comportamientos creados en esta práctica son dinámicos y se basan en una máquina de estados finita jerárquica y dirigida por datos. A continuación el pseudocódigo de las clases que se han implementado para esta práctica (contenidos en Assets\Scripts\IAV):

### StateMachineFactory
Componente que implementa el patrón Factory y usa la reflexión de código para poder crear estados o transiciones a partir de su id
```
static _instance : StateMachineFactory
static Instance : StateMachineFactory { get { return _instance } }

_states : Dictionary<string, Type>
_transitions : Dictionary<string, Type>

function Awake() -> void:
    if _instance not equals null and _instance not equals this
        Destroy(this)
    else
        _instance = this

    _states = new Dictionary<string, Type>()
    _transitions = new Dictionary<string, Type>()

    foreach Type type in GetType().Assembly.GetTypes():
        foreach Attribute attribute in Attribute.GetCustomAttributes(type):
            if attribute is id ID:
                if type.BaseType is typeof(State):
                    _states.Add(ID.ID, type)
                else if type.BaseType is typeof(Transition):
                    _transitions.Add(ID.ID, type)
                
function GetState(name : string) -> State:
    ConstructorInfo ctor = _states[name].GetConstructor()
    State instance = (State)ctor.Invoke()
    return instance

function GetTransition(name : string) -> Transition:
    ConstructorInfo ctor = _transitions[name].GetConstructor();
    Transition instance = (Transition)ctor.Invoke();
    return instance;
```
### StateMachine
Componente para procesar una máquina de estados
```
class StateMachine extends MonoBehaviour
    _definition : StateMachineDefinition
    _current : State
    _states : Dictionary<string, State> //Mapa que guarda la relación entre estados y nombres de estos para el caso de que en una máquina de estados haya más de un estado con el mismo id
    _transitions : Dictionary<State, List<Transitions>>

    function Start() -> void:
        _states = new Dictionary<string, State>()
        _transitions = new Dictionary<State, list<Transitions>()
        
        for i = 0; i < _definition.states.Length; i++:
            state : State = StateMachineFactory.GetState(_definition.states[i])
            _states.push(_definition.names[i], state)
            if state is StateMachine:
                SubStateMachine substate = (SubStateMachine)state
                substate.SetDefinition(EnemyBlackboard.GetDefinition(_definition.names[i]))
            state.Init(gameObject, this, this.EnemyBlackboard)
            _transitions.Add(state, List<Transition>)
        
        for i = 0; i < _definition.origin.Length; i++:
            transition : Transition = StateMachineFactory.GetTransition(_definition.transitions[i])
            _transitions[_states[_definition.origin[i]]].Add(transition)
            transition._origin = _states[_definition.origin[i]]
            transition._dest = _states[_definition.destination[i]]
            transition.Init(gameObject, this, this.EnemyBlackboard)

        _current = _states[_definition.names[0]]
        _current.Enter()


    function Update() -> void:
        _current.Update()

        while i < _transitions[_current].Length and not _transitions[_current][i].Check():
              i++
        if i not equals _transitions[_current].Length:
              _current.Exit()
              _current = _transitions[_current][i]._dest
              _current.Enter()
```
### StateMachineDefinition
Scriptable Object que guarda la información de estados y transiciones para la StateMachine
```
class StateMachineDefinition extends ScriptableObject
    states : string[]
    transitions : string[]
    names : string[]
    origin : string[]
    destination : string[]
    transitions : string[]

    cycle : bool //Para SubMachineState
        
```
### EnemyBlackboard
Pizarra para los enemigos
```
class EnemyBlackboard extends MonoBehaviour
    _seconds : float
    Seconds : float { get { return _seconds } }
    _distance : float
    Distance : float { get { return _distance; } }

    _defs : StateMachineDefinition[]

    State : enum { WANDER, LISTENING, SMELLING, ATTACKING }
    _actualState : State
    ActualState : State { get { return _actualState } }

    _objetive : Vector3
    Objetive : Vector3 {  get { return _objetive; } }

    _smellPoint : SmellPoint
    _player : GameObject

    function GetDefinition(name : string) -> StateMachineDefinition:
        i : int = 0
        while i < _defs.Length and _defs[i].name not equals name:
            ++i
        return i iquals _defs.Length ? null : _defs[i]

    function StartListening(pos : Vector3) -> void:
        if _actualState equals State.ATTACKING:
            return
        _actualState = State.LISTENING
        _objetive = pos

    function StartSmelling(point : SmellPoint) -> void:
        if _actualState equals State.ATTACKING:
            return
        _actualState = State.SMELLING
        UpdateSmell(point)
        _objetive = _smellPoint.position

    function StartAttacking() -> void:
        _actualState = State.ATTACKING
        _objetive = _player.position

    function StartWander() -> void:
        _actualState = State.WANDER
        _objetive = Vector3.zero

    function SetObjetive(pos : Vector3) -> void:
        _objetive = pos

    UpdateSmell(_other : SmellPoint) -> void:
        if _smellPoint equals null or
            _other.Intensity / Vector3.Distance(_other.position, this.position) >
            _smellPoint.Intensity / Vector3.Distance(_smellPoint.position, this.position):
                _smellPoint = _other

    function Start() -> void:
        _actualState = State.WANDER
        _objetive = Vector3.zero
        _player = GameObject.Find("Player")

    function Update() -> void:
        if _actualState equals State.SMELLING and _smellPoint equals null:
            StartWander()
```
### State
Clase de la que heredan todos los estados
```
class id extends Attribute
    _id : string
    ID : string { get {return _id } }
    id(id : string):
        _id = id

[id(string)]
class State extends Attribute
    _gameObject : GameObject
    _machine : StateMachine
    _blackboard : EnemyBlackboard

    function Init( g : GameObject , machine : StateMachine, blackboard : EnemyBlackboard) -> void:
           _gameObject = g
           _machine = machine
           _blackboard = blackboard

    function Enter() -> void
    function Update() -> void
    function Exit() -> void
        
```
#### SubStateMachine
Estado que puede ser el padre de otros estados
```
[id("SubStateMachine")]
class SubStateMachine extends State
    _definition : StateMachineDefinition
    _current : State
    _states : Dictionary<string, State>
    _transitions : Dictionary<State, List<Transitions>>
    _index : int
    _cycle : bool
    _exit : bool
    exit : bool { get { return _exit } }

    function SetDefinition(def : StateMachineDefinition) -> void:
        _definition = def

    function Enter() -> void:
        _states = new Dictionary<string, State>()
        _transitions = new Dictionary<State, list<Transitions>()

        _cycle = _definition.cycle
        _exit = false
        _index = 0
        
        for i = 0; i < _definition.states.Length; i++:
            state : State = StateMachineFactory.GetState(_definition.states[i])
            _states.push(_definition.names[i], state)
            if state is StateMachine:
                SubStateMachine substate = (SubStateMachine)state
                substate.SetDefinition(EnemyBlackboard.GetDefinition(_definition.names[i]))
            state.Init(gameObject, this, this.EnemyBlackboard)
            _transitions.Add(state, List<Transition>)
        
        for i = 0; i < _definition.origin.Length; i++:
            transition : Transition = StateMachineFactory.GetTransition(_definition.transitions[i])
            _transitions[_states[_definition.origin[i]]].Add(transition)
            transition._origin = _states[_definition.origin[i]]
            transition._dest = _states[_definition.destination[i]]
            transition.Init(gameObject, this, this.EnemyBlackboard)

        _current = _states[_definition.names[0]]
        _current.Enter()


    function Update() -> void:
        _current.Update()

        i : int = 0;
        while i < _transitions[_current].Length and !_transitions[_current][i].Check():
            ++i;
        if i not equals _transitions[_current].Length:
            _current.Exit();
            if _index < _transitions.Length - 1 or _cycle
                _current = _transitions[_current][i]._dest
                _current.Enter()
                _index++
            else
                _exit = true
```
#### WaitState
Estado para que espere unos segundos
```
[id("Wait")]
class WaitState extends State
    _seconds : float
    Seconds : float { get { return _seconds } }
    _agent : NavMeshAgent

    elapsedTime : float { get, private set }

    function Enter() -> void:
        elapsedTime = 0;
       _seconds = _blackboard.Seconds
        _agent = _gameObject.GetComponent<NavMeshAgent>()
        _agent.isStopped = true
        _blackboard.SetObjetive(Vector3.zero)
        _blackboard.StartWander()

    function Update() ->void:
        elapsedTime += Time.deltaTime

    function Exit() -> void:
        _agent.isStopped = false
```
#### ChoosePoint
Estado para que elija un punto aleatorio del NavMeshSurface
```
[id("Choose")]
class ChoosePoint extends State
    _position : Vector3
    Position : Vector3 { get { return _position } }
    _agent : NavMeshAgent

    function Enter() -> void:
        _position = Vector3.zero
        _agent = _gameObject.GetComponent<NavMeshAgent>()
        _agent.isStopped = true

    function Update() -> void:
        _position = RandomNavmeshLocation(_blackboard.Distance, _gameObject.position)
        if !_agent.CalculatePath(_position, _agent.path):
            _position = Vector3.zero
        else:
            _blackboard.SetObjetive(_position)

    function RandomNavmeshLocation(radius : float, pos : Vector3 = new Vector3()) -> Vector3:
        randomPoint : Vector3 = pos + Random.insideUnitSphere * radius
        finalPosition : Vector3 = Vector3.zero
        for i : int = 0; i < 30; i++:
            hit : NavMeshHit
            if NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas):
                finalPosition = hit.position
        return finalPosition
```
#### MoveToPoint
Estado para que se mueva hacia un objetivo
```
[id("MovePoint")]
class MoveToPoint extends State
    _agent : NavMeshAgent

    function Enter() -> void:
        _agent = _gameObject.GetComponent<NavMeshAgent>()
        _agent.isStopped = false

    function Update() -> void:
        _agent.SetDestination(_blackboard.Objetive)
```
#### FollowPlayerState
Estado para que siga al jugador
```
[id("Follow")]
class FollowPlayerState extends State
    _player : GameObject
    _agent : NavMeshAgent

    function Enter() -> void:
        _player = GameObject.Find("Player")
        _agent = _gameObject.GetComponent<NavMeshAgent>()
        _agent.speed = 2.0f

    function Update() -> void:
        _blackboard.SetObjetive(_player.transform.position)
        _agent.SetDestination(_player.transform.position)

    function Exit() -> void:
        _agent.speed = 1.0f
```
### Transition
Clase de la que heredan todas las transiciones
```
[id(transition)]
class Transition 
    _gameObject : GameObject
    _machine : StateMachine
    _origin : State
    _dest : State

    function Init( g : GameObject , machine : StateMachine) -> void:
           _gameObject = g
           _machine = machine

    function Check() -> bool:
        return _origin._completed    
```
#### ExitChoose
Transición para salir del estado ChoosePoint
```
[id("ExitChoose")]
class ExitChoose extends Transition
    function Check() -> bool:
        ChoosePoint choose = (ChoosePoint)_origin
        return choose.Position not equals Vector3.zero
```
#### ExitMove
Transición para salir del estado MoveToPoint
```
[id("ExitMove")]
class ExitMove extends Transition
    function Check() -> bool:
        return Vector3.Distance(_gameObject.position, _blackboard.Objetive) <= 0.1
```
#### ExitSubMachine
Transición para salir de una submáquina de estados no cíclica
```
[id("ExitSubMachine")]
class ExitSubMachine extends Transition
    function Check() -> bool:
        SubStateMachine subState = (SubStateMachine)_origin
        return subState.exit
```
#### ExitWait
Transición para salir del estado WaitState
```
[id("ExitWait")]
class ExitWait extends Transition
    function Check() -> bool:
        WaitState wait = (WaitState)_origin
        return wait.elapsedTime > wait.Seconds
```
#### ToAttack
Transición para ir a la submáquina Attack
```
[id("ToAttack")]
class ToAttack extends Transition
    function Check() -> bool:
        return _blackboard.ActualState equals EnemyBlackboard.State.ATTACKING
```
#### ToListen
Transición para ir a la submáquina Listen
```
[id("ToListen")]
class ToListen extends Transition
    function Check() -> bool:
        return _blackboard.ActualState equals EnemyBlackboard.State.LISTENING
```
#### ToSmell
Transición para ir a la submáquina Smell
```
[id("ToSmell")]
class WanderToSmell extends Transition
    function bool Check() -> bool:
        return _blackboard.ActualState equals EnemyBlackboard.State.SMELLING
```
#### ToWander
Transición para ir a la submáquina Wander
```
[id("ToWander")]
class ToWander extends Transition
    function Check() -> bool:
        return _blackboard.ActualState equals EnemyBlackboard.State.WANDER
```
### Smell
Componente que crea puntos de olor
```
class Smell extends MonoBehaviour
    _timeToSmell : float
    _time : float
    _maxDistance : float
    _point : GameObject
    _lastPoint : GameObject

    function Start() -> void:
        _lastPoint = null
        _time = 0

    function Update() -> void:
        _time += Time.deltaTime
        if _time >= _timeToSmell:
            _time = 0
            if _lastPoint equals null or Vector3.Distance(this.position, _lastPoint.position) > _maxDistance:
                _lastPoint = Instantiate(_point, transform.position, Quaternion.identity)
            else:
                _lastPoint.GetComponent<SmellPoint>().ResetIntensity()
```
### SmellPoint
Componente de los puntos de olfato
```
class SmellPoint extends MonoBehaviour
    Intensity : float { get { return _collider.radius } }
    _maxIntensity : float
    _minIntensity : float
    _speed : float
    _collider : SphereCollider

    function ResetIntensity() -> void:
        _collider.radius = _maxIntensity

    function OnTriggerEnter(other : Collider) -> void:
        if other.GetComponent<EnemyBlackboard>() not equals null:
            other.GetComponent<EnemyBlackboard>().StartSmelling(this)

    function Start() -> void:
        _collider = GetComponent<SphereCollider>()
        _collider.radius = _maxIntensity

    function Update() -> void
        _collider.radius -= _speed * Time.deltaTime
        if _collider.radius < _minIntensity:
            Destroy(gameObject)
```
### Vision
Componente que provee del sentido de la visión a los enemigos
```
class Vision extends MonoBehaviour
    _visionPoint : Transform
    _maxDistance : float
    _width : float
    _height : float

    _blackboard : EnemyBlackboard

    function Start() -> void:
        _blackboard = GetComponent<EnemyBlackboard>()

    _layerMask : LayerMask
    function FixedUpdate() -> void:
        pos : Vector3 = new Vector3(transform.position.x, _visionPoint.position.y, transform.position.z)
        obj : Collider[] = Physics.OverlapBox(pos + transform.forward * _maxDistance / 2, new Vector3(_width / 2, _height / 2, _maxDistance / 2), Quaternion.identity, _layerMask)
        if obj.Length > 0:
            _blackboard.StartAttacking()
        else if _blackboard.ActualState equals EnemyBlackboard.State.ATTACKING:
            _blackboard.StartWander()
```
## Pruebas y métricas
Las pruebas a realizar están basadas en los distintos apartados, explicados en Características.

### Prueba del merodeo
Para esta prueba he desactivado el sentido del oído, olfato y visión para comprobar que el enemigo se mueve a un punto aleatorio después de esperar unos segundos
- [Video de la prueba](https://drive.google.com/file/d/10jT01D27uJp1EEsXckTNg3TgcJPPH9Hs/view?usp=sharing)

### Prueba del oído
Para esta prueba he desactivado el olfato y la visión para comprobar que el enemigo se mueve a la fuente del sonido
- [Video de la prueba](https://drive.google.com/file/d/1GAQvCw9AkPP2I7vVNuwCPQlGdU6zZjvC/view?usp=sharing)

### Prueba del olfato
Para esta prueba he desactivado la visión y el oído para comprobar que el enemigo detecta el ratro de olor del jugador y va hacia él
- [Video de la prueba](https://drive.google.com/file/d/1H5Vb30oSMINzB-YU8X40HrNyi3tTaZOW/view?usp=sharing)

### Prueba de la vista
Para esta prueba he desactivado el olfato y el oído para comprobar que cuando el enemigo detecta al jugador va a por él
- [Video de la prueba](https://drive.google.com/file/d/1v-sUy29OMQSuL4aicmQ1JWcK9YJ_HBQ6/view?usp=sharing)

## Video final
- [Video](https://drive.google.com/file/d/1ReBzXPMOHyzyZ_PDBpLl6pAtD3mi6ClE/view?usp=sharing)

## Licencia
Pablo Sánchez Martín, autor de la documentación, código y recursos de este trabajo, no concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

## Referencias

Los recursos de terceros utilizados son de uso público.

- **Assets del escenario**, [Flooded Grounds](https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529)

- **Documentación de Meta XR All-in-One SDK**, [documentación](https://developer.oculus.com/documentation/unity/unity-import-samples/)

