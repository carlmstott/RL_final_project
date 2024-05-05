Link to YouTube Video: https://youtu.be/7b_U3Sds5rA


This is a project built in Unity.

Setup instructions:

This project requires the following software versions:
Unity >= 2022.3.1
Python = 3.9.13

To setup training environment:
Inside the Unity project folder, create a Python virtual environment. Install the following packages:
- mlagents==0.30.0
- torch, torchvision, torchaudio
- protobuf version==3.20.3 (this will be automatically installed as the wrong version above)
- tensorboard

With Unity environment loaded, to simulate training you can first open your virtual environment in the Unity project folder. Then run the command:
"mlagents-learn config/PPO_config.yaml --run-id=ExampleRun"
and click play in the Unity environment when reqested to start training in the Unity environment and then the command:
"tensorboard --logdir results --port 6006"
to view the results of training in realtime using tensorboard. 

The resulting policy is stored in the .onnx file in the results directory (in the first directory layer of the Unity project). This file can be 
copied into the Assets folder and dragged into the "Model" section of "Behavior Parameters". Ensure that "inference device" is set to "inference only"
and then click the Play button (without the training command) to view the resulting policy. 

Note: There are two scenes in the project. A training scene, with several examples of the project to run training in parallel and a scene with a 
single example to more easily view the results of a policy. 




