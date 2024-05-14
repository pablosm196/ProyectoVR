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

Con lo proporcionado como base se deberá plantear una IA que permita a los zombies moverse por el entorno y encontrar al jugador con su percepción. El comportamiento de los zombies será creado a partir de una máquina de estados finita (FSM).

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

**Jugadorr:**
- Movimiento simple mediante el **joystick izquierdo**
- Rotación contínua mediante el **joystick derecho**
- Disparo mediante el **gatillo primario derecho**
- Interactuar con ciertos objetos mediante el **gatillo secundario izquierdo**

**Guardias/Enemigos:**
- Merodeo aleatorio por el escenario
- Capacidad de detectar al enemigo dentro de un cono de visión.
- Al detectar al personaje comienza a perseguirle.
    * En caso de perder de vista al jugador, volverá al merodeo aleatorio.
- Capacidad de escuchar los disparos del jugador.
    * Al escuchar un disparo, irá a la posición de la fuente del sonido.
- Capacidad de oler al jugador.
    * Los zombies podrán oler el rastro de olor que va dejando un jugador.

## Diagrama de comportamientos

Todas los comportamientos creados en esta práctica son dinámicos y se basan en una máquina de estados finita jerárquica y dirigida por datos. A continuación el pseudocódigo de las clases que se han implementado para esta práctica:

### Apartado A: Máquina de estados finita 
#### StateMachine
```
class StateMachine extends MonoBehaviour
    _definition : StateMachineDefinition
    _current : State
    _transitions : Dictionary (State, list<Transitions>)

    function Start() -> void
        _transitions = new Dictionary (State, list<Transitions>)
        foreach State s in  _definition.States:
            s.Init(...)
        foreach Transition t in  _definition.Transition:
            t.Init(...)
            _transitions[t._origin].add(t)
        _current = _definition.States[0]
        _current.Enter()

    function Update() -> void
        _current.Update()
        while i < _transitions[_current].size() y __transitions[_current][i] no ha cumplido el check()
              i++
        si i diferente de _transitions[_current].size()
              _current.Exit()
              _current = _transitions[_current][i]._dest
              _current.Enter()
```
#### StateMachineDefinition
Scriptable Object que guarda la información de stados y transiciones para la StateMachine
```
class StateMachineDefinition extends ScriptableObject
   States : State[]
   Transitions : Transition[]
        
```
#### State
Clase de la que heredan todos los estados
```
class State extends ScriptableObject
    _gameObject : GameObject
    _machine : StateMachine

    function Init( g : GameObject , machine : StateMachine) -> void
           _gameObject = g
           _machine = machine

    function Enter() -> void
    function Update() -> void
    function Exit() -> void
        
```
##### SubStateMachine
Estado que puede ser el padre de otros estados
```
class SubStateMachine extends State
    _definition : StateMachineDefinition
    _current : State
    _transitions : Dictionary (State, list<Transitions>)

    function Start() -> void
        _transitions = new Dictionary (State, list<Transitions>)
        foreach State s in  _definition.States:
            s.Init(...)
        foreach Transition t in  _definition.Transition:
            t.Init(...)
            _transitions[t._origin].add(t)
        _current = _definition.States[0]
        _current.Enter()

    function Update() -> void
        _current.Update()
        while i < _transitions[_current].size() y __transitions[_current][i] no ha cumplido el check()
              i++
        si i diferente de _transitions[_current].size()
              _current.Exit()
              _current = _transitions[_current][i]._dest
              _current.Enter()
```
##### ChooseNextWaypoint
Estado que elige el siguiente punto

```
class ChooseNextWaypoint extends State
    initialWaypoint : GameObject

    currentWaypoint : GameObject = null

    nextWaypoint : GameObject = null
    
    function Enter() -> void:
        if currentWaypoint is null:
            if initialWaypoint is null:
                return
            currentWaypoint = initialWaypoint
            nextWaypoint = initialWaypoint
        else
            currentWaypoint = nextWaypoint
            nextWaypoint = currentWaypoint.nextWaypoint
```
##### MoveToGameObject
Estado en el que se mueve a una entidad
```
class MoveToGameObject extends State
    _target : GameObject
    _closeDistance : float
    _lockToFirstGameObjectPosition : bool

    _navAgent : NavMeshAgent
    _targetTransform : Transform

    function Enter() -> void:
        if _target is null:
            return

        _targetTransform = _target.transform

        _navAgent = _gameObject.NavMeshAgent
        if _navAgent is null:
            _navAgent = _gameObject.AddComponent<NavMeshAgent>()

        path : NavMeshPath = new NavMeshPath();
        if _navAgent.CalculatePath(_targetTransform.position, path):
            corners = path.corners
            fullDistance : float = 0f

            for int i = 1; i < corners.Length; i++:
                fullDistance += Distance(corners[i - 1], corners[i])

            if fullDistance > _closeDistance:
                _navAgent.SetDestination(_targetTransform.position)


            if UNITY_5_6_OR_NEWER:
                _navAgent.isStopped = false;
            else:
                navAgent.Resume();

    function void Update() -> void:
        if not _lockToFirstGameObjectPosition and _navAgent.destination not equals _targetTransform.position:
            _navAgent.SetDestination(_targetTransform.position)

    function Exit() -> void:
        if UNITY_5_6_OR_NEWER:
            if _navAgent not equals null:
                _navAgent.isStopped = true
            else:
                if navAgent not equals null
                    navAgent.Stop()
```
##### RotateRandomly
Estado en el que la entidad rota aleatoriamente
```
class RotateRandomly extends State
    _rotationSpeed : float
    _acceptableRange : float = 0.1f
    _goalRotation : Quaterion

    function void Enter() -> void:
        direction : int =  Random(-1, 1) >= 0 ? 1 : -1
        _goalRotation = Quaternion.Euler(0, Random(90f, 180f) * direction, 0);

    function Update() -> void:
        currentAngularDistance : float = Quaternion.Angle(_gameObject.rotation, _goalRotation);

        t : float = ClampBetween0And1(deltaTime * _rotationSpeed / currentAngularDistance);

        _gameObject.rotation =
            Quaternion.Slerp(_gameObject.rotation, _goalRotation, t);    
```
##### ToggleAnimator
Estado en el establece el estado de una animación
```
class ToggleAnimator extends State
    _enableAnimator : bool;

    _finished : bool

    _animator : Animator

    function void Enter() -> void:
        _animator = _gameObject.Animator
        _finished = false

    function void Update() -> void:
        if _animator not equals null:
            _animator.enabled = _enableAnimator
        _finished = true
```
##### WaitForSeconds
Estado en el que se queda unos segundos
```
class WaitForSeconds extends State
    _seconds : float

    elapsedTime : float

    function void Enter() -> void:
        elapsedTime = 0

    function void Update() -> void:
        elapsedTime += deltaTime
```
#### Transition
Clase de la que heredan todas las transiciones
```
class Transition extends ScriptableObject
    _gameObject : GameObject
    _machine : StateMachine
    _origin : State
    _dest : State

    function Init( g : GameObject , machine : StateMachine) -> void
           _gameObject = g
           _machine = machine

    function Check() -> bool        
```
##### ChooseToMove
Transición entre ChooseNextWaypoint y MoveToGameObject
```
class ChooseToMove extends Transition
    function bool Check() -> bool:
        choose : ChooseNextWaypoint = (ChooseNextWaypoint)_origin
        move : MoveToGameObject = (MoveToGameObject)_dest
        
        if choose.currentWaypoint not equals null
            move._target = choose.nextWaypoint

        return choose.currentWaypoint not equals null
```
##### MoveToRotate
Transición entre MoveToGameObjects y RotateRandom
```
class MoveToRotate extends Transition
    function bool Check() -> bool:
        move : MoveToGameObject = (MoveToGameObject)_origin
        return move._navAgent.destination is null or move._navAgent.remainingDistance <= Max(move._navAgent.stoppingDistance, move._closeDistance)  
```
##### RotateToWait
Transición entre RotateRandom y WaitForSeconds
```
class RotateToWait extends Transition
    function bool Check() -> bool:
        rotate : RotateRandom = (RotateRandom)_origin

        var dot = Quaternion.Dot(rotate._goalRotation, _gameObject.transform.rotation);
        var abs = Abs(dot);
        return 1 - abs <= rotate._acceptableRange
```
##### WaitToToggle
Transición entre WaitForSeconds y ToggleAnimator
Transición entre ToggleAnimator y ChooseNextWaypoint
```
class WaitToToggle extends Transition
    function bool Check() -> bool:
        wait : WaitForSeconds = (WaitForSeconds)_origin
        return wait.elapsedTime >= wait._seconds
```
##### ToggleToChoose
Transición entre ToggleAnimator y ChooseNextWaypoint
```
class ToggleToChoose extends Transition
    function bool Check() -> bool:
        s : ToggleAnimator = (ToggleAnimator)_origin
        return s._finished
```

## Pruebas y métricas
Las pruebas a realizar están basadas en los distintos apartados, explicados en Características.

### Prueba A Escenario y movimiento del jugador y cámara
Probamos que el jugador se mueve correctamente por el escenario y comprobamos que la cámara le sigue cuando se mueve. También probamos que la cámara es capaz de aplicar un zoom que nos permite obtener una visión general del entorno.

- [Próximamente: Vídeo de la prueba A]()

### Prueba B Obstáculos y visibilidad de los enemigos
Probamos que las puertas impiden el paso tanto de los enemigos como de Néstor. También probamos que los enemigos no son capaces de detectar a Néstor si éste está detrás de una puerta. Por útimo comprobamos que los enemigos no detectan a Néstor si éste está en un "escondite".

- [Próximamente: Vídeo de la prueba B]()

### Prueba C Enemigos
Comprobamos que los enemigos realizan su ruta de patrulla. Al detectar a Néstor, probamos que la abandonan para perseguirle y dispararle. Probamos que si consiguen matarle, la escena se reinicia. Y si le pierden de vista regresan a su ruta. Por último, probamos que si se quedan sin munición vuelven a su base a recargar.

- [Próximamente: Video de la prueba C]()

### Prueba D Jugador 
Comprobamos que el jugador intenta alcanzar la salida desde distintos puntos de la manera más óptima y evitando a los enemigos.

- [Próximamente: Video de la prueba D]()

### Prueba E: 
Comprobamos que Néstor aplica la información previa proporcionada al comenzar la partida.

-[Próximamente: Video de la prueba F]()

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores.

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
| ✔ | Read me |17-04-2024|
|   |  | |
|  | OPCIONAL |  |

## Licencia
Pablo Sánchez Martín, autor de la documentación, código y recursos de este trabajo, no concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

## Referencias

Los recursos de terceros utilizados son de uso público.

- **Assets del escenario**, [Flooded Grounds](https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529)

