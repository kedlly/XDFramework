from sqlalchemy import Column, Integer, String, create_engine, Date, Boolean
from sqlalchemy.orm import sessionmaker

from Configure import DATA_SOURCE_NAME

engine = create_engine(DATA_SOURCE_NAME, echo=False)
DBSession = sessionmaker(bind=engine)