// ���� ifdef ���Ǵ���ʹ�� DLL �������򵥵�
// ��ı�׼�������� DLL �е������ļ��������������϶���� P2PNET_EXPORTS
// ���ű���ġ���ʹ�ô� DLL ��
// �κ�������Ŀ�ϲ�Ӧ����˷��š�������Դ�ļ��а������ļ����κ�������Ŀ���Ὣ
// P2PNET_API ������Ϊ�Ǵ� DLL ����ģ����� DLL ���ô˺궨���
// ������Ϊ�Ǳ������ġ�
#ifdef P2PNET_EXPORTS
#define P2PNET_API __declspec(dllexport)
#else
#define P2PNET_API __declspec(dllimport)
#endif

// �����Ǵ� P2pNet.dll ������
class P2PNET_API CP2pNet {
public:
	CP2pNet(void);
	// TODO: �ڴ�������ķ�����
};

extern P2PNET_API int nP2pNet;

P2PNET_API int fnP2pNet(void);
