# pixel_walker
### Table Of Content
[About](#description)  

[Requirement](#requirement)

[How To Use](#manual)

[Application Content](#content)

### About the Project<a name="description"/>
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
  <strong>Window:   </strong>    Minimum Win 7, recommend 10 or better <br>
  <strong>Processor:    </strong>    Intel(R) Core(TM) i7-6650U CPU @ 2.20GHz   2.21 GHz <br>
  <strong>RAM:    </strong>    8GB or better <br>
  <strong>Graphic Card:   </strong>    Minimum Intel(R) Iris(R) Graphics 550 
</section> <br>

### Manual - How To Use Pixel Walker<a name="manual"/>
<section>
    <strong># Program Menu: </strong> <br>
    - Give the user options to load the virual environment to interact with the AI agent <br> 
    - Or Store the api key from the server so that only the main user can access the app <br> <br>
    <strong> App Display: </strong> <br>
    - User can type in input at the bottom bar so that the agent can either answer a question or perform an action <br>
    - The answer text will be display at the top right box with an indication <br>
    - User can move around the view to follow the agent by click and drag the screen <br>
    
</section>

### Application Content<a name="content"/>
<section>
  <strong>C#</strong> --- for Scripts, Classes, Methods <br>
  <strong>Unity3D</strong> --- for environment design, app-running platform <br>
  <strong>ML-agents</strong> --- for training the agent, deep learning using the reward system <br>
  <strong>GPT-3</strong> --- the building block of the app, handling prompts <br>
</section>

