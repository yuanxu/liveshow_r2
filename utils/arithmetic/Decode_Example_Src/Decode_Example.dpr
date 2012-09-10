library Decode_Example;

{ Important note about DLL memory management: ShareMem must be the
  first unit in your library's USES clause AND your project's (select
  Project-View Source) USES clause if your DLL exports any procedures or
  functions that pass strings as parameters or function results. This
  applies to all strings passed to and from your DLL--even those that
  are nested in records and classes. ShareMem is the interface unit to
  the BORLNDMM.DLL shared memory manager, which must be deployed along
  with your DLL. To avoid using BORLNDMM.DLL, pass string information
  using PChar or ShortString parameters. }

{$R *.res}

//Ω‚√‹
function Plugin_Decode(pIn: pChar; Size: LongWord; pOut: pChar): LongWord;stdcall;
var
  i : Integer;
  TmpByte : Byte;
begin
  for i:=0 to Size do
  begin
    TmpByte:=ord(pIn[i])-1;
    pOut[i]:=Chr(TmpByte);
    Result:=i;
  end;
end;

exports
  Plugin_Decode;

begin
end.
 