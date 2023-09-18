import os
#from llama_index import SimpleDirectoryReader, VectorStoreIndex

def CreateDataFolder():
    # Get the full path to the current script
    script_path = os.path.abspath(__file__)
    # Get the directory containing the script
    script_dir = os.path.dirname(script_path)
    # Remove the '.py' extension from the script's filename and append 'IndexData'
    data_folder_name = os.path.splitext(os.path.basename(script_path))[0] + "IndexData"
    # Create the full path to the data folder
    full_data_folder_path = os.path.join(script_dir, data_folder_name)
    # Check if the folder exists
    if not os.path.exists(full_data_folder_path):
        # Create the folder if it doesn't exist
        os.makedirs(full_data_folder_path)

# # Initialize the index using the dynamically created or found folder
# documents = SimpleDirectoryReader(full_data_folder_path).load_data()
# index = VectorStoreIndex.from_documents(documents)

# def query_index(query_str):
#     query_engine = index.as_query_engine()
#     response = query_engine.query(query_str)
#     return str(response)
