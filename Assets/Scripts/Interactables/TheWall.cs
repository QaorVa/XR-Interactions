using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

[ExecuteAlways]
public class TheWall : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField] int columns;
    [SerializeField] int rows;

    [SerializeField] GameObject wallCubePrefab;
    [SerializeField] GameObject socketWallPrefab;
    [SerializeField] int socketPosition;

    [SerializeField] XRSocketInteractor wallSocket;
    public XRSocketInteractor GetWallSocket => wallSocket;


    [SerializeField] ExplosiveDevice explosiveDevice;
    [SerializeField] List<GeneratedColumn> generatedColumns;
    GameObject[] wallCubes;
    [SerializeField] float cubeSpacing = 0.005f;
    private Vector3 cubeSize;
    private Vector3 spawnPosition;

    [SerializeField] GameObject areaToEnable;

    [SerializeField] bool buildWall;
    [SerializeField] bool deleteWall;
    [SerializeField] bool destroyWall;
    [SerializeField] int maxPower;

    [SerializeField] AudioClip destroyWallClip;
    public AudioClip GetDestroyClip => destroyWallClip;
    [SerializeField] AudioClip socketClip;
    public AudioClip GetSocketClip => socketClip;
    void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnSocketEnter);
            wallSocket.selectExited.AddListener(OnSocketExited);

        }
        if(explosiveDevice != null)
        {
            explosiveDevice.OnDetonated.AddListener(OnDestroyWall);
        }


    }

    private void OnSocketExited(SelectExitEventArgs arg0)
    {
        if (generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].ResetColumn();
            }
        }
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        /*areaToEnable.SetActive(true);*/
        
        if (generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].ActivateColumn();
            }
        }
        
    }

    private void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        spawnPosition = transform.position;

        int socketedColumn = Random.Range(0, columns);
        for (int i = 0; i < columns; i++)
        {
            if (i == socketedColumn)
            {
                GenerateColumn(i, rows, true);
            } else
            {
                GenerateColumn(i, rows, false);
            }
                
            
            spawnPosition.x += cubeSize.x + cubeSpacing;
        }
        
    }

    private void GenerateColumn(int index, int height, bool socketed)
    {
        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializedColumn(transform, index, height, socketed);

        spawnPosition.y = transform.position.y;

        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length;i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPosition, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }

        if (socketed && socketWallPrefab != null)
        {
            if(socketPosition < 0 || socketPosition >= height)
            {
                socketPosition = 0;
            }

            AddSocketWall(tempColumn);
           
        }
        generatedColumns.Add(tempColumn);
        
    }

    private void AddSocketWall(GeneratedColumn socketedColumn)
    {
        if (wallCubes[socketPosition] != null)
        {
            Vector3 wallCubePosition = wallCubes[socketPosition].transform.position;
            DestroyImmediate(wallCubes[socketPosition]);

            wallCubes[socketPosition] = Instantiate(socketWallPrefab, wallCubePosition, transform.rotation);

            socketedColumn.SetCube(wallCubes[socketPosition]);

            if (socketPosition == 0)
            {
                wallCubes[socketPosition].transform.SetParent(transform);
            }
            else
            {
                wallCubes[socketPosition].transform.SetParent(wallCubes[0].transform);
            }

            wallSocket = wallCubes[socketPosition].GetComponentInChildren<XRSocketInteractor>();

            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(OnSocketEnter);
                wallSocket.selectExited.AddListener(OnSocketExited);

            }


        }
    }

    private void OnDestroyWall()
    {
        OnDestroy?.Invoke();
        if (generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                int power = Random.Range(maxPower / 2, maxPower);

                generatedColumns[i].DestroyColumn(power);
            }
        }
        areaToEnable.SetActive(true);
    }
    

    

    // Update is called once per frame
    void Update()
    {
        if(buildWall)
        {
            buildWall = false;
            BuildWall();
        }

        if(deleteWall)
        {
            deleteWall = false;
            for(int i =0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].DeleteColumn();
            }
            generatedColumns.Clear();
        }
    }

    [System.Serializable]
    public class GeneratedColumn
    {
        [SerializeField] GameObject[] wallCubes;
        [SerializeField] bool isSocketed;
        [SerializeField] int columnIndex;

        private bool isParented;
        private Transform parentObject;
        private Transform columnObject;
        private const string COLUMN_NAME = "Column";
        private const string SOCKETED_COLUMN_NAME = "Socketed Column";
        public void InitializedColumn(Transform parent, int index, int rows, bool socketed)
        {
            columnIndex = index;
            parentObject = parent;
            wallCubes = new GameObject[rows];
            isSocketed = socketed;
        }

        public void SetCube(GameObject cube)
        {
            for(int i = 0; i < wallCubes.Length; i++)
            {
                if(!isParented)
                {
                    isParented = true;
                    SetColumnName(cube, columnIndex);
                    cube.transform.SetParent(parentObject);
                    columnObject = cube.transform;
                    
                } else
                {
                    cube.transform.SetParent(columnObject);
                }

                if (wallCubes[i] == null)
                {
                    wallCubes[i] = cube;
                    return;
                }
            }
        }

        public void DeleteColumn()
        {
            for (int i = 0; i < wallCubes.Length; i++)
            {
                if (wallCubes[i] != null)
                {
                    Object.DestroyImmediate(wallCubes[i]);
                }
            }
            wallCubes = new GameObject[0];
        }

        public void DestroyColumn(int power)
        {
            
            for (int i = 0; i < wallCubes.Length;i++)
            {
                if (wallCubes[i] != null)
                {
                    Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.constraints = RigidbodyConstraints.None;
                    wallCubes[i].transform.SetParent(parentObject);
                    
                    rb.AddRelativeForce(Random.onUnitSphere * power);
                }
            }
        }
        public void ActivateColumn()
        {
            for (int i = 0; i < wallCubes.Length; i++)
            {
                if (wallCubes[i] != null)
                {
                    Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                    rb.isKinematic = false;

                }
            }
        }

        public void ResetColumn()
        {
            foreach (var wallCube in wallCubes)
            {
                if (wallCube != null)
                {
                    Rigidbody rb = wallCube.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
            }
        }

        public void SetColumnName(GameObject column, int index)
        {
            if(isSocketed)
            {
                column.name = SOCKETED_COLUMN_NAME;
            } else
            {
                column.name = COLUMN_NAME;
            }

            column.name += " " + (index + 1);
        }
    }
}
