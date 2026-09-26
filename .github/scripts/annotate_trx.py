"""Turn failed tests in TRX reports into GitHub Actions error annotations,
so failures show up on the pull request without opening the raw log."""

import pathlib
import sys
import xml.etree.ElementTree as ET

NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


def main(results_dir: str) -> None:
    reports = list(pathlib.Path(results_dir).rglob("*.trx"))
    if not reports:
        print("::warning::No TRX reports found; tests may have failed before running.")
        return

    for report in reports:
        root = ET.parse(report).getroot()
        for result in root.iterfind(".//t:UnitTestResult", NS):
            if result.get("outcome") != "Failed":
                continue
            message = result.findtext(".//t:ErrorInfo/t:Message", default="", namespaces=NS)
            text = f"{result.get('testName')}: {message}".strip()
            print("::error title=Test failed::" + text.replace("%", "%25").replace("\r", "").replace("\n", "%0A"))


if __name__ == "__main__":
    main(sys.argv[1] if len(sys.argv) > 1 else "TestResults")
