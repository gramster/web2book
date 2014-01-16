#pragma once

__declspec(dllexport) int Connect(char *path);
__declspec(dllexport) int GetNext(char *path, char filename[], int space);
__declspec(dllexport) int Delete(char *path);
__declspec(dllexport) int Copy(char *src, char *dest);
__declspec(dllexport) void Disconnect(void);

