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
        id : string[] = State.CustomAttributes
        foreach State s in  _definition.States:
            state : State = new id[s.id]
            state.Init(...)
        foreach Transition t in  _definition.Transition:
            transition : Transition = new id[t.id]
            transition.Init(...)
            _transitions[transition._origin].add(transition)
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
Scriptable Object que guarda la información de estados y transiciones para la StateMachine
```
class StateMachineDefinition extends ScriptableObject
   States : State[]
   Transitions : Transition[]
        
```
#### State
Clase de la que heredan todos los estados
```
class id extends Attribute
    _id : string

[id(state)]
class State extends Attribute
    _gameObject : GameObject
    _machine : StateMachine
    _completed : bool

    function Init( g : GameObject , machine : StateMachine) -> void:
           _gameObject = g
           _machine = machine
           _completed = false

    function Enter() -> void
    function Update() -> void
    function Exit() -> void
        
```
##### SubStateMachine
Estado que puede ser el padre de otros estados
```
[id(SubState)]
class SubStateMachine extends State
    _definition : StateMachineDefinition
    _current : State
    _transitions : Dictionary (State, list<Transitions>)

    function Start() -> void:
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
#### Transition
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

## Pruebas y métricas
Las pruebas a realizar están basadas en los distintos apartados, explicados en Características.


## Licencia
Pablo Sánchez Martín, autor de la documentación, código y recursos de este trabajo, no concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

## Referencias

Los recursos de terceros utilizados son de uso público.

- **Assets del escenario**, [Flooded Grounds](https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529)

- **Documentación de Meta XR All-in-One SDK**, [documentación](https://developer.oculus.com/documentation/unity/unity-import-samples/)

