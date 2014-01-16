// LBWrapper.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"


#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

class __declspec(dllimport) CLBParser {
public:
  CLBParser();
  virtual ~CLBParser();
  int AddPage(char *,int,class CLRFPageObject *);
  int AddPage(struct IHTMLDocument2 *,int,class CLRFPageObject *);
  int AddPage(unsigned short const *,int,class CLRFPageObject *);
  int AffairBook();
  int CreateNewBook();
  int FinalizeBook();
  int OpenBook();
  int SetBookAuthor(unsigned short const *);
  int SetBookAuthor(char *);
  int SetBookAuthor_read(unsigned short const *);
  int SetBookAuthor_read(char *);
  int SetBookID(unsigned short const *);
  int SetBookID(char *);
  int SetBookTitle(unsigned short const *);
  int SetBookTitle(char *);
  int SetBookTitle_read(unsigned short const *);
  int SetBookTitle_read(char *);
  int SetDesignFileName(unsigned short const *);
  int SetDesignFileName(char *);
  virtual int SetDocument(char const *);
  virtual int SetDocument(unsigned short *);
  int SetLRFFileName(char *);
  int SetLRFFileName(unsigned short const *);
  int SetTocTitle(char *);
  int SetTocTitle(unsigned short const *);
  void setDivID(unsigned short const *);
  void setHTMLStartComment(unsigned short const *);
  void setHTMLEndComment(unsigned short const *);
  void setHTMLStartComment(char *);
  void setHTMLEndComment(char *);
  void setHTMLEndExceptComment(unsigned short const *);
  void setHTMLStartExceptComment(unsigned short const *);
private:
  int createCashDir();
  int removeCashDir();
  char filler[0x98];
}; //size = 0x9C

CLBParser parser;
FILE *debugfp = 0;

extern "C" {
__declspec(dllexport) 
void StartBook(char * title, char *author, char *fname)
{
	debugfp = fopen("c:\\debug.txt", "w");
	fprintf(debugfp, "Setting title to %s\n", title);
	parser.SetBookTitle(title);
    parser.SetBookTitle_read(title);
	fprintf(debugfp, "Setting author to %s\n", author);
    parser.SetBookAuthor(author);
    parser.SetBookAuthor_read(author);
	fprintf(debugfp, "Setting output file to %s\n", fname);
	parser.SetLRFFileName(fname);
	fprintf(debugfp, "Setting design file\n"); fflush(debugfp);
    parser.SetDesignFileName("DesignHorizontal.lrf");
    fprintf(debugfp, "Creating new book\n"); fflush(debugfp);
    parser.CreateNewBook();
    /*char bookID[17];
    time_t now = time(0);
    strftime(bookID,16,"FBENNP%m%d%H%M%S",localtime(&now));
    bookID[16]=0;
	fprintf(debugfp, "Setting id to %s\n", bookID);
    parser->SetBookID(bookID);*/
}

__declspec(dllexport)
void AddContent(char *html_fname)
{
	fprintf(debugfp, "Adding content file %s\n", html_fname);
	fclose(debugfp);
	parser.AddPage(html_fname, 0, 0);
}

__declspec(dllexport) 
void FinishBook()
{
    parser.FinalizeBook();
}

}


