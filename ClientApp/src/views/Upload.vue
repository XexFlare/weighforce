<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <h2>Upload Report</h2>
    <br />
    <div
      class="
        bg-green-50
        border-dashed border-4 border-light-green-500
        p-4
        rounded
      "
    >
      <dl>
        <dt>
          <label for="report">File</label>
        </dt>
        <dd>
          <input
            type="file"
            ref="file"
            name="report"
            id="report"
            v-on:change="onChangeFileUpload($event.target.files)"
          />
        </dd>
      </dl>
      <br />
      <button
        @click="upload()"
        class="bg-accent mt-4 px-2 py-1 text-white rounded hover:bg-primary"
      >
        Upload
      </button>
    </div>
    <div class="mt-4">
      <output name="result">Status: {{ status }}</output>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  inject: ["isAuthenticated", "authStringHeader"],
  data() {
    return {
      status: "",
      files: new FormData(),
    };
  },
  methods: {
    async upload() {
      this.status = "Upload In Progress";
      axios
        .post(
          `${import.meta.env.VITE_API}/api/bookings/import/excel`,
          this.files,
          {
            headers: {
              "Content-Type": "multipart/form-data",
              ...(await this.authStringHeader()),
            },
          }
        )
        // var formData = this.files;
        // fetch(`${import.meta.env.VITE_API}/api/bookings/import/excel`, {
        //   method: "POST",
        //   headers: {
        //     // Accept: "application/json",
        //     "Content-Type": "multipart/form-data",
        //   },
        //   body: formData,
        // })
        .then((res) => {
          console.log();
          if (res.data.count >= 1) {
            this.status = "Upload Complete";
          }
          return res;
        })
        .catch((error) => {
          this.status = "Upload Failed";

          console.error("Unable to upload file.", error);
        });
    },
    onChangeFileUpload(fileList) {
      this.files = new FormData();
      this.files.append("file", fileList[0], fileList[0].name);
    },
  },
};
</script>