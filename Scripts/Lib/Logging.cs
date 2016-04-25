using UnityEngine;
using System.Collections;
using System.IO;
using VoltUtil;
using Logger = VoltUtil.Logger;

public class Logging : MonoBehaviour
{
	private StreamWriter _writer;
	private Logger _logger;

	void Awake() {
		_writer = new StreamWriter (Path.Combine (Application.dataPath, ".logs/test.log"), true);
		_writer.AutoFlush = true;

		_logger = new Logger(_writer, true);
		//Application.RegisterLogCallback (HandleLog);

	}

	void Start () {
		_logger.Log(LogLevel.INFO, "Started!");
	}

	void Update () {
	
	}
}
