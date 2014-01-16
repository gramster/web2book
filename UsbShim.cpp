// UsbShim.cpp : Defines the entry point for the DLL application.

#include "stdafx.h"
#include "UsbShim.h"

#define USE_DYNAMIC_LINKING	1

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

//---------------------------------------------------------------

HINSTANCE readerLib = 0;

typedef int (*UsbUnlockDeviceFn)(char* v);
typedef int (*UsbConnectFn)();
typedef int (*UsbDisConnectFn)();
typedef int (*UsbInitCheckFn)();
typedef int (*UsbGetProtcolVerFn)(int x, char *buff);
typedef int (*UsbReceiveProcFn)(void* request, size_t answersize, void* answer);
typedef int (*UsbSendProcFn)(void* request, void *sendBuffer, size_t sendsize, DWORD* bytesSent);
typedef void (*UsbBuffFreeFn)(void* answer);

UsbUnlockDeviceFn	MyUsbUnlockDevice = 0;
UsbConnectFn		MyUsbConnect = 0;
UsbDisConnectFn		MyUsbDisConnect = 0;
UsbInitCheckFn		MyUsbInitCheck = 0;
UsbGetProtcolVerFn	MyUsbGetProtcolVer = 0;
UsbReceiveProcFn	MyUsbReceiveProc = 0;
UsbSendProcFn		MyUsbSendProc = 0;
UsbBuffFreeFn		MyUsbBuffFree = 0;

#if DEBUG
FILE *logfp=0;
#endif

void ShowError(LPCSTR lpszFunction) 
{ 
    char szBuf[200]; 
    LPVOID lpMsgBuf;
    DWORD dw = GetLastError(); 

    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER | 
        FORMAT_MESSAGE_FROM_SYSTEM,
        NULL,
        dw,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPSTR) &lpMsgBuf,
        0, NULL );

    sprintf(szBuf, 
        "%s failed with error %d: %s", 
        lpszFunction, dw, lpMsgBuf); 
 
    MessageBox(NULL, szBuf, "Error", MB_OK); 
	//fprintf(logfp, "%s\n", szBuf);

    LocalFree(lpMsgBuf);
}

#if USE_DYNAMIC_LINKING

int InitReaderDll()
{
#if DEBUG
	logfp = fopen("c:\\usbshim.log", "w");
#endif

	if (readerLib == 0)
	{
		readerLib = LoadLibrary("ebookUsb.dll"); 
		if (readerLib != 0)
		{
#if DEBUG
			fprintf(logfp, "Opened Sony reader DLL");
#endif
			MyUsbUnlockDevice = (UsbUnlockDeviceFn)GetProcAddress(readerLib, "_UsbUnlockDevice@4");
			MyUsbConnect = (UsbConnectFn)GetProcAddress(readerLib, "_UsbConnect@0");
			MyUsbDisConnect = (UsbDisConnectFn)GetProcAddress(readerLib, "_UsbDisConnect@0");
			MyUsbInitCheck = (UsbInitCheckFn)GetProcAddress(readerLib, "_UsbInitCheck@0");
			MyUsbGetProtcolVer = (UsbGetProtcolVerFn)GetProcAddress(readerLib, "_UsbGetProtcolVer@8");
			MyUsbReceiveProc = (UsbReceiveProcFn)GetProcAddress(readerLib, "_UsbReceiveProc@12");
			MyUsbSendProc = (UsbSendProcFn)GetProcAddress(readerLib, "_UsbSendProc@16");
			MyUsbBuffFree = (UsbBuffFreeFn)GetProcAddress(readerLib, "_UsbBuffFree@4");
//			if (MyUsbBuffFree != 0) fprintf(logfp, "Got function pointers\n");
		}
		else
		{
#if DEBUG
			fprintf(logfp, "Failed to open Sony reader DLL");
#endif
			ShowError("LoadLibrary");
		}
	}
	return (MyUsbBuffFree == 0) ? -1 : 0;
}

#else

extern int _stdcall _UsbUnlockDevice(char* v);
extern int _stdcall _UsbConnect();
extern int _stdcall _UsbDisConnect();
extern int _stdcall _UsbInitCheck();
extern int _stdcall _UsbGetProtcolVer(int x, char *buff);
extern int _stdcall _UsbReceiveProc(void* request, size_t answersize, void* answer);
extern int _stdcall _UsbSendProc(void* request, void *sendBuffer, size_t sendsize, DWORD* bytesSent);
extern void _stdcall _UsbBuffFree(void* answer);

int InitReaderDll()
{
	logfp = fopen("c:\\usbshim.log", "w");

	MyUsbUnlockDevice = (UsbUnlockDeviceFn)_UsbUnlockDevice;
	MyUsbConnect = (UsbConnectFn)_UsbConnect;
	MyUsbDisConnect = (UsbDisConnectFn)_UsbDisConnect;
	MyUsbInitCheck = (UsbInitCheckFn)_UsbInitCheck;
	MyUsbGetProtcolVer = (UsbGetProtcolVerFn)_UsbGetProtcolVer;
	MyUsbReceiveProc = (UsbReceiveProcFn)_UsbReceiveProc;
	MyUsbSendProc = (UsbSendProcFn)_UsbSendProc;
	MyUsbBuffFree = (UsbBuffFreeFn)_UsbBuffFree;
	if (MyUsbBuffFree != 0) fprintf(logfp, "Got function pointers\n");
	return (MyUsbBuffFree == 0) ? -1 : 0;
}

#endif

void ReleaseReaderDll()
{
#if USE_DYNAMIC_LINKING
	if (readerLib != 0)
	{
		FreeLibrary(readerLib);
		readerLib = 0;
		MyUsbUnlockDevice = 0;
		MyUsbConnect = 0;
		MyUsbDisConnect = 0;
		MyUsbInitCheck = 0;
		MyUsbGetProtcolVer = 0;
		MyUsbReceiveProc = 0;
		MyUsbSendProc = 0;
		MyUsbBuffFree = 0;
	}
#endif
#if DEBUG
	if (logfp != 0) fclose(logfp);
	logfp=0;
#endif
}

//------------------------------------------------------------------

#define REQ_FILE_OPEN		0x10
#define REQ_FILE_CLOSE		0x11
#define REQ_GET_FILE_SIZE	0x12
#define REQ_SET_FILE_SIZE	0x13
#define REQ_FILE_READ		0x16
#define REQ_FILE_WRITE		0x17
#define REQ_GET_FILE_INFO	0x18
#define REQ_FILE_CREATE		0x1A
#define REQ_FILE_DELETE		0x1B
#define REQ_DIR_ENUM_START	0x33
#define REQ_DIR_ENUM_STOP	0x34
#define REQ_DIR_ENUM_NEXT	0x35

//----------------------------------------------------
// Request structures
#pragma pack(1)

typedef struct _Request 
{
 DWORD reqNo;
 DWORD reserved[2];
 DWORD extralen;
} Request;

typedef struct _PathRequestExtra // requests that take a path argument
{
   DWORD nPathLen;
   char path[0]; // Path must follow
} PathRequestExtra;

typedef struct _PathRequest // requests that take a path argument
{
   Request req;
   PathRequestExtra extra;
} PathRequest;


typedef struct _HandleRequestExtra // requests that take a handle argument
{
    DWORD handle;
 } HandleRequestExtra;

typedef struct _HandleRequest // requests that take a handle argument
{
	Request req;
    HandleRequestExtra extra;
 } HandleRequest;

typedef struct _FileOpenRequestExtra // requests that take a path argument
{
   DWORD mode; // 0 = read, 1 = write
   DWORD nPathLen;
   char path[0]; // Path must follow
} FileOpenRequestExtra;

typedef struct _FileOpenRequest // requests that take a path argument
{
   Request req;
   FileOpenRequestExtra extra;
} FileOpenRequest;

typedef struct _ReadWriteRequestExtra
{
    DWORD handle;
    long long offset;
    DWORD size;
} ReadWriteRequestExtra;

typedef struct _WriteRequest
{
	Request req;
	ReadWriteRequestExtra extra;
} WriteRequest;

typedef struct _SendBuffer
{
	DWORD buffType;
	DWORD reserved[2];
	DWORD dataLen;
	char data[0];
} SendBuffer;

//----------------------------------------------------
// Answer structures

typedef struct _Answer
{
 DWORD reserved[3];
 DWORD dataLen;
 //char  data[1]; //variable length
} Answer;


typedef struct _HandleAnswerExtra
{
    DWORD hFile;
} HandleAnswerExtra;

typedef struct _HandleAnswer
{
    Answer ans;
    HandleAnswerExtra extra;
} HandleAnswer;

typedef struct _DirEnumAnswerExtra
{
    DWORD nType; //1=file,2=dir
    DWORD nPathLen;
    char path[1];//of nPathLen bytes
} DirEnumAnswerExtra;

typedef struct _DirEnumAnswer
{
	Answer ans;
	DirEnumAnswerExtra extra;
} DirEnumAnswer;

//---------------------------------------------------
// Request 'constructors'

Request *AllocRequest(int reqNum, int size)
{
	Request *req = (Request*)malloc(size);
	req->reqNo = reqNum;
	req->reserved[0] = req->reserved[1] = 0;
	req->extralen = size - sizeof(Request);
	return req;
}

PathRequest *MakePathRequest(int reqNum, char *path)
{
	int n = (int)strlen(path);
	PathRequest *req = (PathRequest*)AllocRequest(reqNum, sizeof(PathRequest)+n);
	req->extra.nPathLen = n;
	memcpy(req->extra.path, path, n);
	return req;
}

HandleRequest *MakeHandleRequest(int reqNum, int handle)
{
	HandleRequest *req = (HandleRequest*)AllocRequest(reqNum, sizeof(HandleRequest));
	req->extra.handle = handle;
	return req;
}

FileOpenRequest *MakeOpenRequest(int reqNum, int mode, char *path)
{
	int n = (int)strlen(path);
	FileOpenRequest *req = (FileOpenRequest*)AllocRequest(reqNum, sizeof(FileOpenRequest)+n);
	req->extra.mode = mode;
	req->extra.nPathLen = n;
	memcpy(req->extra.path, path, n);
	return req;
}


WriteRequest *MakeWriteRequest(int reqNum, DWORD handle, DWORD size)
{
	WriteRequest *req = (WriteRequest*)AllocRequest(reqNum, sizeof(WriteRequest));
	req->extra.handle = handle;
	req->extra.offset = 0;
	req->extra.size = size;
	return req;
}


//------------------------------------------------------

int SimplePathRequest(int reqNum, char *path)
{
	PathRequest *req = MakePathRequest(reqNum, path);
	Answer* ans = 0;
	int rtn = -1;
	if ((*MyUsbReceiveProc)(req, 0, &ans) >= 0)
	{
		rtn = 0;
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}

int FileCreate(char *path)
{
	return SimplePathRequest(REQ_FILE_CREATE, path);
}

int FileOpen(char *path, int mode)
{
	int rtn = -1;
	FileOpenRequest* req = MakeOpenRequest(REQ_FILE_OPEN, mode, path);
	HandleAnswer *ans = 0;
	if ((*MyUsbReceiveProc)(req, sizeof(HandleAnswerExtra), &ans) == 0)
	{
		rtn = ans->extra.hFile;
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}

int FileWrite(DWORD handle, void *buf, int len)
{
	int rtn = -1;
    WriteRequest* req = MakeWriteRequest(REQ_FILE_WRITE, handle, len);
	DWORD written = 0;
	SendBuffer *sb = (SendBuffer*)malloc(sizeof(SendBuffer)+len);
	sb->buffType = 0x10005;
	sb->reserved[0] = sb->reserved[1] = 0;
	sb->dataLen = len;
	memcpy(sb->data, buf, len);
	if ((*MyUsbSendProc)(req, sb, len, &written) >= 0)
	{
		rtn = written;
	}
	free(req);
	return rtn;
}

int FileClose(DWORD handle)
{
	int rtn = -1;
	HandleRequest* req = MakeHandleRequest(REQ_FILE_CLOSE, handle);
	Answer *ans = 0;
	if ((*MyUsbReceiveProc)(req, 0, &ans) == 0)
	{
		rtn = 0;
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}

int ReaderReadDirectoryStart(char *path)
{
	PathRequest *req = MakePathRequest(REQ_DIR_ENUM_START, path);
	HandleAnswer* ans = 0;
	int rtn = -1;
	if ((*MyUsbReceiveProc)(req, sizeof(HandleAnswerExtra), &ans) == 0)
	{
		rtn = ans->extra.hFile;
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}

int ReaderReadDirectoryStop(int handle)
{
	int rtn = -1;
	HandleRequest* req = MakeHandleRequest(REQ_DIR_ENUM_STOP, handle);
	Answer *ans = 0;
	if ((*MyUsbReceiveProc)(req, 0, &ans) == 0)
	{
		rtn = 0;
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}

char *ReaderReadDirectoryNext(int handle)
{
	char *rtn = 0;
	HandleRequest* req = MakeHandleRequest(REQ_DIR_ENUM_NEXT, handle);
	DirEnumAnswer *ans = 0;
	if ((*MyUsbReceiveProc)(req, sizeof(DirEnumAnswerExtra), &ans) == 0)
	{
		rtn = (char*)malloc(ans->extra.nPathLen+1);
		strncpy(rtn, ans->extra.path, ans->extra.nPathLen);
		rtn[ans->extra.nPathLen] = 0;
	}
	else
	{
		ReaderReadDirectoryStop(handle);
	}
	if (ans != 0) (*MyUsbBuffFree)(ans);
	free(req);
	return rtn;
}


//-----------------------------------------------------
// Exported entry points

int connected = 0;

__declspec(dllexport) int Connect(char *readerPath)
{
	(void)SetDllDirectoryA((LPCSTR)readerPath);

	if (InitReaderDll() < 0) return -1;

    if (!connected)
	{
		char ver[8];
		if ((*MyUsbInitCheck)() == 0 && (*MyUsbGetProtcolVer)(1, ver) == 0 && (*MyUsbUnlockDevice)("-1") == 0 && (*MyUsbConnect)() == 0)
		{
			connected = 1;
		}
	}

	if (!connected) ReleaseReaderDll();

	return connected ? 0 : -1;
}

__declspec(dllexport) void Disconnect(void)
{
    if (connected) (*MyUsbDisConnect)();
    connected = 0;
	ReleaseReaderDll();
}

__declspec(dllexport) int Delete(char *path)
{
	return SimplePathRequest(REQ_FILE_DELETE, path);
}

int dirHandle = -1;

__declspec(dllexport) int GetNext(char *path, char filename[], int space)
{
	int rtn = -1;
	if (dirHandle < 0) dirHandle = ReaderReadDirectoryStart(path);
	if (dirHandle >= 0)
	{
		char *f = ReaderReadDirectoryNext(dirHandle);
		if (f == 0)
		{
			dirHandle = -1;
		}
		else
		{
			int fn = strlen(f);
			if (fn >= space)
			{
				ReaderReadDirectoryStop(dirHandle);
				dirHandle = -1;
				rtn = -fn;
			}
			else
			{
				rtn = 0;
				strcpy(filename, f);
			}
		}
	}
	return rtn;
}

__declspec(dllexport) int Copy(char *src, char *dest)
{
	int rtn = -1;
	FILE *fp = fopen(src, "rb");
	if (fp != 0)
	{
		size_t len;
		char *buf;

		// Get file size; could use stat() I suppose

		fseek(fp, 0l, SEEK_END);
		len = ftell(fp);
		fseek(fp, 0l, SEEK_SET);
		buf = (char*)malloc(len);
		fread(buf, 1, len, fp);
		fclose(fp);
#if DEBUG
		fprintf(logfp, "Attempting to create dest file %s\n", dest);
#endif
		if (FileCreate(dest) >= 0)
		{
			int h;
#if DEBUG
			fprintf(logfp, "Attempting to open dest file %s for writing\n", dest);
#endif
			h = FileOpen(dest, 1);
			if (h != -1)
			{
#if DEBUG
				fprintf(logfp, "Attempting to write %d bytes to dest file %s\n", len, dest);
#endif

				if (FileWrite(h, buf, len) == len)
				{
#if DEBUG
					fprintf(logfp, "Success!\n");
#endif
					rtn = 0;
				}
				FileClose(h);
			}
			if (rtn < 0) Delete(dest); // delete partial file
		}
		free(buf);
	}
	else
	{
#if DEBUG
		fprintf(logfp, "Couldn't open source file %s\n", src);
#endif
	}
	return rtn;
}
