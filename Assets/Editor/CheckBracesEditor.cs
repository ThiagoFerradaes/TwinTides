using UnityEngine;
using UnityEditor;
using System.IO;

public class CheckBracesEditor : EditorWindow {
    [MenuItem("Tools/Check Braces in Assets")]
    public static void ShowWindow() {
        GetWindow<CheckBracesEditor>("Check Braces");
    }

    private void OnGUI() {
        GUILayout.Label("Verificar arquivos com chaves desbalanceadas", EditorStyles.boldLabel);

        if (GUILayout.Button("Verificação linha por linha")) {
            CheckBraces();
        }

        if (GUILayout.Button("Verificar arquivos curtos (erro na linha 2)")) {
            CheckEarlyBraceErrors();
        }
    }

    private static void CheckBraces() {
        string assetsPath = Application.dataPath;
        string[] extensions = new[] { ".cs", ".json", ".shader", ".uss", ".txt", ".asmdef", ".asset" };
        int totalFilesChecked = 0;
        int totalProblems = 0;

        Debug.Log("Iniciando verificação de chaves (linha por linha)...");

        foreach (var file in Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories)) {
            if (!System.Array.Exists(extensions, ext => file.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)))
                continue;

            totalFilesChecked++;
            string[] lines = File.ReadAllLines(file);
            int openCount = 0, closeCount = 0;

            bool inString = false;
            bool inSingleLineComment = false;
            bool inMultiLineComment = false;

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++) {
                string line = lines[lineIndex];
                for (int i = 0; i < line.Length; i++) {
                    char c = line[i];

                    if (c == '"' && !inSingleLineComment && !inMultiLineComment) {
                        inString = !inString;
                        continue;
                    }

                    if (!inString && !inMultiLineComment && c == '/' && i + 1 < line.Length && line[i + 1] == '/') {
                        inSingleLineComment = true;
                        break;
                    }

                    if (!inString && !inMultiLineComment && c == '/' && i + 1 < line.Length && line[i + 1] == '*') {
                        inMultiLineComment = true;
                        i++;
                        continue;
                    }

                    if (inMultiLineComment && c == '*' && i + 1 < line.Length && line[i + 1] == '/') {
                        inMultiLineComment = false;
                        i++;
                        continue;
                    }

                    if (!inString && !inSingleLineComment && !inMultiLineComment) {
                        if (c == '{') openCount++;
                        else if (c == '}') closeCount++;

                        if (closeCount > openCount) {
                            string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
                            Debug.LogError($"⚠️ Chave `}}` a mais no arquivo: {relativePath}, linha {lineIndex + 1}");
                            totalProblems++;
                            goto NextFile;
                        }
                    }
                }

                inSingleLineComment = false;
            }

            if (openCount > closeCount) {
                string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
                Debug.LogError($"⚠️ Faltando `}}` no final do arquivo: {relativePath} (abertas: {openCount}, fechadas: {closeCount})");
                totalProblems++;
            }

        NextFile:
            continue;
        }

        Debug.Log($"✅ Verificação concluída. Arquivos analisados: {totalFilesChecked}. Arquivos com problemas: {totalProblems}.");
    }

    private static void CheckEarlyBraceErrors() {
        string assetsPath = Application.dataPath;
        string[] extensions = new[] { ".json", ".asmdef", ".asset" };
        int totalChecked = 0;
        int totalFound = 0;

        Debug.Log("Iniciando verificação de arquivos curtos (2 linhas)...");

        foreach (var file in Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories)) {
            if (!System.Array.Exists(extensions, ext => file.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)))
                continue;

            string[] lines = File.ReadAllLines(file);
            if (lines.Length < 3) {
                totalChecked++;
                int open = 0, close = 0;
                foreach (string line in lines) {
                    foreach (char c in line) {
                        if (c == '{') open++;
                        else if (c == '}') close++;
                    }
                }

                if (open != close) {
                    totalFound++;
                    string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
                    Debug.LogError($"⚠️ Suspeita de erro de parser na linha 2: {relativePath} (abertas: {open}, fechadas: {close})");
                }
            }
        }

        Debug.Log($"✅ Verificação concluída. Arquivos curtos verificados: {totalChecked}. Problemas encontrados: {totalFound}.");
    }
}



