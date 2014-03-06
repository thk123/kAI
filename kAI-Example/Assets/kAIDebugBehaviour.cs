using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Winterdom.IO.FileMap;
using kAI.Core;
using ThreadMessaging;
using kAI.Core.Debug;
using MemoryMappedFile = Winterdom.IO.FileMap.MemoryMappedFile;

[RequireComponent(typeof(kAIUnityBehaviour))]
public class kAIDebugBehaviour : MonoBehaviour {

    kAIXmlBehaviour behaviour;
    kAIBehaviourDebugStore store;

    static int nextId = 0;
    static int updateIndex = 0;
    int myId;
    bool updatedLastFrame = false;

    /*MemoryMappedFile debugFile;

    kAIXmlBehaviourDebugInfo DebugInfo;
	ProcessSemaphore semaphore = null;*/
	// Use this for initialization
	void Start () 
	{
        behaviour = GetComponent<kAIUnityBehaviour>().XmlBehaviour;
        if (!kAIDebugServer.IsInit)
        {
            print("Init Debug Server");
            kAIDebugServer.Init("Unity" + System.Diagnostics.Process.GetCurrentProcess().Id);
        }

        store =  kAIDebugServer.AddBehaviour(behaviour, gameObject.name);

        myId = nextId;
        ++nextId;
	}
		
	// Update is called once per frame
	void Update () 
	{
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Break();
        }

        store.Update();/*
        if (store != null)
        {
            if (updateIndex == myId)
            {
                if (updatedLastFrame)
                {
                    ++updateIndex;
                    updateIndex = updateIndex % nextId;
                    updatedLastFrame = false;
                }
                else
                {
                    
                    updatedLastFrame = true;
                }


            }
        }*/
		/*semaphore.Acquire();
        Stream stream = debugFile.MapView(MapAccess.FileMapWrite, 0, 1024 * 1024 * 1024);*/
        
        //BinaryFormatter writer = new BinaryFormatter();
      //  Stream stream2 = new FileStream("C:\\test.bin", FileMode.OpenOrCreate);
        /*StreamWriter twriter = new StreamWriter("E:\\test2.txt");
        BinaryFormatter writer = new BinaryFormatter();
        Sertool.Sertool.OutputSerializationInformation(behaviour, twriter);
        twriter.Close();*/
        //print(twriter.GetStringBuilder().ToString());
        /*DebugInfo = (kAIXmlBehaviourDebugInfo)behaviour.GenerateDebugInfo();
        writer.Serialize(stream, DebugInfo);
        stream.Flush();
        stream.Close();*/

        /*using (Stream inStream = MemoryMappedFile.Open(MapAccess.FileMapRead, "Debug").MapView(MapAccess.FileMapRead, 0, 1024 * 1024 * 1024))
        {
            BinaryFormatter vf = new BinaryFormatter();
            //object something = writer.Deserialize(inStream);
            kAIBehaviour b = (kAIBehaviour)vf.Deserialize(inStream);
             Console.WriteLine(b.BehaviourID);
        }*/
		//semaphore.Release();

	}

	void HandleException(Exception e)
	{
		print (e);
		print (e.InnerException);
		print (e.Message);
		Debug.Break();
	}
}
