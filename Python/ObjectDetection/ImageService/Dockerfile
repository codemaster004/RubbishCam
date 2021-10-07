FROM python:3.8-slim-buster

WORKDIR /app

COPY requirements.txt requirements.txt
RUN pip install -r requirements.txt
COPY ./src ./src

EXPOSE 5000

#ENTRYPOINT ["python"]
#CMD ["/app/src/upload_file.py" ]
