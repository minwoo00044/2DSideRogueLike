import os
import csv

# ------------------- 설정 -------------------
# 1. 알려주신 SPUM 패키지 경로로 수정했습니다.
ROOT_DIRECTORY = "C:/MYNewProject/2DSideRogueLike/Assets/SPUM/Resources/Addons"

# 2. 결과를 저장할 CSV 파일의 이름을 지정하세요. (경로에 맞게 수정)
OUTPUT_CSV_FILE = "spum_addons_asset_list.csv"
# -------------------------------------------

def generate_asset_list():
    """
    지정된 디렉토리에서 SPUM 에셋을 찾아 목록을 CSV 파일로 저장합니다.
    """
    asset_data_list = []
    
    print(f"'{ROOT_DIRECTORY}' 폴더에서 에셋 스캔을 시작합니다...")

    # os.walk를 사용하여 모든 하위 폴더를 탐색합니다.
    for root, dirs, files in os.walk(ROOT_DIRECTORY):
        for filename in files:
            # .png 파일만 대상으로 하고, Unity가 생성하는 .meta 파일은 무시합니다.
            if filename.lower().endswith('.png'):
                
                full_path = os.path.join(root, filename)
                
                # 파일명에서 확장자를 제거하여 에셋 이름을 얻습니다.
                asset_name = os.path.splitext(filename)[0]
                
                # 부모 폴더의 이름을 파츠 타입으로 사용합니다.
                part_type = os.path.basename(root)
                
                # Unity의 Resources.Load에서 사용할 경로를 생성합니다.
                try:
                    # os.sep은 운영체제에 맞는 경로 구분자입니다. (Windows: \\, Mac/Linux: /)
                    resource_path_full = root.split('Resources' + os.sep)[1]
                    resource_path = os.path.join(resource_path_full, asset_name).replace("\\", "/")
                except IndexError:
                    resource_path = "N/A (Resources 폴더 밖에 있음)"

                asset_data_list.append({
                    "Name": asset_name,
                    "PartType": part_type,
                    "ResourcePath": resource_path,
                    "FullPath": full_path.replace("\\", "/")
                })

    if not asset_data_list:
        print("에셋을 찾을 수 없습니다. ROOT_DIRECTORY 경로를 확인해주세요.")
        return

    print(f"총 {len(asset_data_list)}개의 에셋을 찾았습니다. '{OUTPUT_CSV_FILE}' 파일로 저장합니다.")

    # CSV 파일로 저장
    try:
        with open(OUTPUT_CSV_FILE, 'w', newline='', encoding='utf-8-sig') as csvfile:
            writer = csv.DictWriter(csvfile, fieldnames=asset_data_list[0].keys())
            writer.writeheader()
            writer.writerows(asset_data_list)
        print("성공적으로 저장했습니다!")
    except Exception as e:
        print(f"파일 저장 중 오류가 발생했습니다: {e}")


if __name__ == "__main__":
    generate_asset_list()