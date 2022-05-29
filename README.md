# pixel_walker
### Table Of Content
[About](#description)  

[Requirement](#requirement)

[How To Use](#manual)

[Application Content](#content)

### About the Application<a name="description"/>
<section>
Pixel Walker is an application about a virtual actor in a virtual environment that is trained to recognize, classify, and act upon commands given by the application user. The user will input a sentence string into our program, upon which it will be parsed by GPT-3, a natural language processor. This result will then be fed into Unity3D wherein the virtual actor, which has been trained several select actions using ML-agents, will perform actions implied by the user’s input string in a three-dimensional virtual world space, simulating a home garage environment.
</section> <br>

<section>
Many robots that complete tasks only follow motion data copied from a person performing actions. We want to see more dynamic responses in a non-static environment, training the agent to teach itself to solve problems instead of having us supply the solution data for them. This idea could be a small contribution towards AI gaining the autonomy to solve problems on its own. We hope this will sparks curiosity from young developers interested in reinforcement learning.
</section> <br>

<section>
# Key Terms to Note: <br>

·    Unity3D – A game engine in which we will create and simulate our virtual environment and agents.

·    ML-agents - A tool in unity that uses reinforcement learning to train virtual agents to perform actions.

·    GPT-3 – A neural network machine uses machine learning to create human like texts. 
</section> <br>

<section>
Users can interact with the virtual actor by typing in phrases, commands, or questions. The virtual actor will then go through a training regimen provided by the ML-agents so it is ready to respond to the user's input. If it is a command, the command parser will handle the command input and make the actor initiate the requested behavior. Else if it is a question, the question responder will handle the question input and generate the most appropriate answer. Else if it is a regular phrase, the app will respond with an answer as in a normal conversation. 
</section> <br>

<section>
About commands, the virtual actor can walk to objects, pick up objects, set objects down, open and close objects as pre-trained actions. The more complex and multi-step actions such as sit down, dance, or use techonology will require more trainings from the system. 
  </section>

### System Requirement<a name="requirement"/>
<section>
  <strong>Window:   </strong>    Recommend Window 10 or better <br>
  <strong>Processor:    </strong>    Intel(R) Core(TM) i7-6650U CPU @ 2.20GHz   2.21 GHz <br>
  <strong>RAM:    </strong>    8GB or better <br>
  <strong>Graphic Card:   </strong>    Minimum Intel(R) Iris(R) Graphics 550 
</section> <br>

### Manual - How To Use Pixel Walker<a name="manual"/>
<section>
    <strong># App Startup: </strong> <br>
    - User will be requested to provide an API key to load a virtual environment. 
      Any invalid key will not load up the app to ensure that user's data is secured. <br>
<img src="images/AppStartupScreen.png">
    <strong># App Home Screen: </strong> <br>
    - User will be loaded into an empty space for the first time and there will be a menu and a reset button to the top left.<br> 
    - User will be able to enter any input into the bottom bar and will receive the output depend on the type. <br>
    - - If it is a command, there will be a command report at the first box to the top right <br>
    - - If it is a question or conversation, there will be a response from Dave in the second box to the right <br>
<img src="images/AppHomeScreen(empty).jpg">
    - The Menu button will lead to another screen presenting options to load the scene, reset, change API key or the engine <br>
    - The Reset button will simply reset the environment along with any object present similarly to the reload the scene option. <br>
<img src="images/AppMenuScreen.png">
    <strong># App Home Screen (with agent): </strong> <br>
    - After user has select a scene, a virtual environment will be loaded with many objects and an AI agent. <br>
    - This environment will stay on even after the app is closed provided that user has the same API key. <br>
    - Here any command entered will set the agent to initiate the requested action. <br>
    - If the command is too difficult, the agent will respond with an excuse of not knowing how to do it. <br>
    - The Cancel button is meant to stop any current action and put the agent in place. <br>
<img src="images/AppHomeScreen(loaded).png">
</section>

### Application Content<a name="content"/>
<section>
  <strong>------------------------------------ C# Scripts -----------------------------------</strong> <br>
  ------------------ <strong> Camera </strong> ------------------ <br>
  <strong>CameraTarget.cs</strong> : Improve scene hierarchy and Agent <br>
  <strong>SimpleCameraController.cs</strong> : Move the camera around the scene with many features
  such as zoom in-out, change angle, etc. <br>
  ------------------------------------------------------------------------------------ <br>
  ------------------ <strong> Character </strong> ------------------ <br>
  <strong>BehaviorController.cs</strong> : <br>
  <strong>CharacterRoot.cs</strong> : <br>
  <strong>ChildRbCollisionListener.cs</strong> : <br>
  <strong>CollisionThrower.cs</strong> : <br>
  <strong>ReferenceOrientation.cs</strong> : <br>
  ------------------------------------------------------------------------------------ <br>
  ------------------ <strong> Prompt/GUI </strong> ------------------ <br>
  <strong>RuntimeGUI.cs </strong>   :   Integrate handling prompts from file with GUI <br>
  <strong>Gpt3Connection.cs</strong>   :   Connect to the GPT-3 main server to run with the app <br>
  <strong>InputHandlerStructure.cs</strong> : The structure of the input handler method <br>
  <strong>PromptLoader.cs</strong> : Load the prompts from file to generate responses <br>
  <strong>UserInputHandler.cs</strong> : The method of handling input <br>
  <br>
  <strong>------------------------------------ Unity3D -----------------------------------</strong> <br>
  - Build the environment, objects, UI <br>
  - <br>
  <strong>------------------------------------ ML-agents -----------------------------------</strong> <br>
  - Train the agent using deep learning <br>
  - Reward points for every positive behavior <br>
  - <br>
  <strong>------------------------------------ GPT-3 -----------------------------------</strong> <br>
  - The building block of the app - process any user input and generate desirable output <br>
  - Handle prompts using the GPT-3 engine (default: Davinci) <br>
  - <br>
</section>

