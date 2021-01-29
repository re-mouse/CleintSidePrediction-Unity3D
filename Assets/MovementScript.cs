using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MovementScript : NetworkBehaviour
{
    List<InputState> sendedInputs = new List<InputState>();

    float totalOffset = 0f;

    private const float MOVEMENT_SPEED = 0.15f;

    private int currentFrame;

    private struct InputState
    {
        public bool ForwardKeyDown;
        public bool BackKeyDown;
        public bool RightKeyDown;
        public bool LeftKeyDown;

        public int frame;

        public static bool operator !=(InputState c1, InputState c2)
        {
            if (c1.ForwardKeyDown != c2.ForwardKeyDown || c1.BackKeyDown != c2.BackKeyDown
                || c1.RightKeyDown != c2.RightKeyDown || c1.LeftKeyDown != c2.LeftKeyDown)
                return true;
            return false;
        }

        public static bool operator ==(InputState c1, InputState c2)
        {
            if (c1.ForwardKeyDown != c2.ForwardKeyDown || c1.BackKeyDown != c2.BackKeyDown
                || c1.RightKeyDown != c2.RightKeyDown || c1.LeftKeyDown != c2.LeftKeyDown)
                return false;
            return true;
        }
    };

    private InputState GetInput()
    {
        InputState currentInput = new InputState();
        currentInput.ForwardKeyDown = Input.GetKey(KeyCode.W);
        currentInput.BackKeyDown = Input.GetKey(KeyCode.S);
        currentInput.RightKeyDown = Input.GetKey(KeyCode.D);
        currentInput.LeftKeyDown = Input.GetKey(KeyCode.A);
        currentInput.frame = currentFrame;
        return currentInput;
    }

    private void Start()
    {
        currentFrame = 0;
    }

    private Vector2 InputToVector2(InputState input)
    {
        Vector2 move = Vector2.zero;
        move.x += input.RightKeyDown ? 1 : 0;
        move.x += input.LeftKeyDown ? -1 : 0;
        move.y += input.ForwardKeyDown ? 1 : 0;
        move.y += input.BackKeyDown ? -1 : 0;
        return move;
    }

    private void ApplyInput(InputState input)
    {
        Physics.autoSimulation = false;
        Vector3 newVector = Vector3.zero;
        newVector.x = InputToVector2(input).x;
        newVector.z = InputToVector2(input).y;
        transform.Translate(newVector * MOVEMENT_SPEED);
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;
    }

    [Command]
    private void UpdateInputState(InputState newInputState)
    {
        ApplyInput(newInputState);
        CheckPrediction(connectionToClient, newInputState, transform.position);
    }

    [TargetRpc]
    private void CheckPrediction(NetworkConnection con, InputState input, Vector3 pos)
    {
        Vector3 temp = transform.position;
        transform.position = pos;
        sendedInputs.Remove(input);
        sendedInputs.ForEach(ApplyInput);
        if (temp != transform.position)
        {
            print("offset = " + Mathf.Abs((temp - transform.position).magnitude));
            totalOffset += Mathf.Abs((temp - transform.position).magnitude);
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        InputState currentInput = GetInput();
        if (InputToVector2(currentInput) != Vector2.zero)
        {
            sendedInputs.Add(currentInput);
            UpdateInputState(currentInput);
            ApplyInput(currentInput);
        }
        currentFrame++;
    }
}
