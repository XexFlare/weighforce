<template>
  <div class="p-6 mb-4" v-if="dispatch">
    <div
      id="details"
      style="margin-right: 10px; margin-left: 10px; margin-bottom: 10px;"
    >
      <div style="display: flex; justify-content: center">
        <img :src="logo.value" alt="" style="height: 70px;" />
        <img
          src="../assets/meridian.jpg"
          alt=""
          style="height: 70px; margin-left: 20px;"
        />
      </div>
      <h2 style="text-align: center">Dispatch Details</h2>
      <table>
        <tr>
          <td style="padding-bottom: 0;">
            <h3 class="text-lg font-bold">
              {{ dispatch.booking?.transportInstruction?.product?.name }}
            </h3>
            <p>
              <b>From:</b>
              {{ dispatch.booking?.transportInstruction?.fromLocation?.name }} -
              <b>To:</b>
              {{ dispatch.booking?.transportInstruction?.toLocation?.name }}
            </p>
            <p>
              <b>LPO Number:</b>
              {{ dispatch.booking?.lpo }}
            </p>
          </td>
          <td style="padding-top: 0; padding-bottom: 0;">
            <h3 class="text-lg font-bold">
              Contract:
              {{
                dispatch.booking?.transportInstruction?.contract?.contractNumber
              }}
            </h3>
            <h3>
              Ticket No:
              <span class="text-primary">{{
                dispatch.initialWeight?.ticketNumber?.substring(
                  0,
                  dispatch.initialWeight.ticketNumber.length - 2
                )
              }}</span>
            </h3>
            <p>
              Description: {{ dispatch.booking?.description }}
            </p>
            <p v-if="dispatch.booking?.loadingAuthorityNumber">
              Loading Authority Number: {{ dispatch.booking?.loadingAuthorityNumber }}
            </p>
          </td>
        </tr>
        <tr>
          <td colspan="2" style="padding-top: 0; padding-bottom: 0;">
            <h3 class="font-bold">
              Truck: {{ dispatch.booking.numberPlate
              }}{{
                dispatch.booking.trailerNumber
                  ? "/" + dispatch.booking.trailerNumber
                  : ""
              }}
            </h3>
          </td>
        </tr>
        <tr>
          <td style="padding-bottom: 0;">
            <b>Driver</b>
          </td>
        </tr>
        <tr style="background: #f3f4f6">
          <td>
            <w-item title="Name" :value="dispatch.booking.driverName" />
            <w-item
              title="Phone Number"
              :value="dispatch.booking.phoneNumber"
            />
          </td>
          <td>
            <w-item
              title="Driver License"
              :value="dispatch.booking.passportNumber"
            />
            <w-item title="Transporter" :value="dispatch.booking.transporter" />
          </td>
        </tr>
        <tr v-if="dispatch.initialWeight.gross > 0">
          <td style="padding-bottom: 0;">
            <b>Dispatch</b>
          </td>
        </tr>
        <tr style="background: #f3f4f6" v-if="dispatch.initialWeight.gross > 0">
          <td style="padding-bottom: 0;">
            <w-item
              title="Tare"
              :value="dispatch.initialWeight.tare.toLocaleString() + ' kg'"
              class="font-bold"
            />
            <w-item
              title="Gross"
              :value="dispatch.initialWeight.gross.toLocaleString() + ' kg'"
              class="font-bold"
            />
          </td>
          <td style="padding-bottom: 0;">
            <w-item
              title="Nett"
              :value="
                (
                  dispatch.initialWeight.gross - dispatch.initialWeight.tare
                ).toLocaleString() + ' kg'
              "
              class="font-bold"
            />
          </td>
        </tr>
        <tr style="background: #f3f4f6" v-if="dispatch.initialWeight.gross > 0">
          <td style="padding-top: 0">
            <w-item
              title="Tare By"
              :value="dispatch.initialWeight.tareUser?.name"
            />
            <w-item
              title="Time In"
              :value="dispatch.initialWeight.tareAt"
              :isDate="true"
            />
          </td>
          <td style="padding-top: 0">
            <w-item
              title="Gross By"
              :value="dispatch.initialWeight.grossUser?.name"
            />
            <w-item
              title="Time Out"
              :value="dispatch.initialWeight.grossAt"
              :isDate="true"
            />
          </td>
        </tr>

        <tr v-if="dispatch.receivalWeight.gross > 0">
          <td style="padding-bottom: 0;">
            <b>Receival</b>
          </td>
        </tr>
        <tr
          v-if="dispatch.receivalWeight.gross > 0"
          style="background: #f3f4f6"
        >
          <td style="padding-bottom: 0;">
            <w-item
              title="Tare"
              :value="dispatch.receivalWeight.tare.toLocaleString() + ' kg'"
              class="font-bold"
            />
            <w-item
              title="Gross"
              :value="dispatch.receivalWeight.gross.toLocaleString() + ' kg'"
              class="font-bold"
            />
          </td>
          <td style="padding-bottom: 0;">
            <w-item
              title="Nett"
              :value="(
                  dispatch.receivalWeight.gross - dispatch.receivalWeight.tare
                ).toLocaleString() + ' kg'"
              class="font-bold"
            />
          </td>
        </tr>
        <tr style="background: #f3f4f6">
          <td style="padding-top: 0;">
            <w-item
              title="Tare By"
              :value="dispatch.receivalWeight.tareUser?.name"
            />
            <w-item
              title="Time In"
              :value="dispatch.receivalWeight.tareAt"
              :isDate="true"
            />
          </td>
          <td style="padding-top: 0;">
            <w-item
              title="Gross By"
              :value="dispatch.receivalWeight.grossUser?.name"
            />
            <w-item
              title="Time Out"
              :value="dispatch.receivalWeight.grossAt"
              :isDate="true"
            />
          </td>
        </tr>
        <br>
        <tr
          v-if="
            dispatch.initialWeight.gross > 0 &&
            dispatch.receivalWeight.gross > 0
          "
          style="background: #f3f4f6"
        >
          <td colspan="2">
            <w-item title="Difference" :value="diff" class="font-bold" />
          </td>
        </tr>
        <tr
          v-if="
            dispatch.firstWeight.gross != null &&
            dispatch.receivalWeight.gross > 0
          "
          style="background: #f3f4f6"
        >
          <td colspan="2">
            <w-item title="First Weighed Gross" :value="dispatch.firstWeight.gross" class="font-bold" />
            <w-item title="First Weighed Gross Difference" :value="dispatch.firstWeight.gross - dispatch.receivalWeight.gross" class="font-bold" />
            <w-item title="First Weighed Tare" :value="dispatch.firstWeight.tare" class="font-bold" />
            <w-item title="First Weighed Tare Difference" :value="dispatch.firstWeight.tare - dispatch.receivalWeight.tare" class="font-bold" />
            <w-item title="First Weighed Date" :value="dispatch.firstWeight.grossAt" :isDate="true" class="font-bold" />
          </td>
        </tr>
      </table>
    </div>
    <w-button v-print="'#details'">Print</w-button>
  </div>
</template>
<style scoped>
table {
  width: 100%;
}
#details {
  font-family: Courier;
  font-size: 14px;
  font-weight: 600;
}
td {
  padding: 12px;
}
</style>
<script>
import WButton from "./WButton.vue";
import print from "vue3-print-nb";
import WItem from "./WItem.vue";
export default {
  props: ["dispatch"],
  inject: ["logo"],
  components: {
    WItem,
    WButton,
  },
  directives: {
    print,
  },
  computed: {
    diff() {
      const received =
        this.dispatch.receivalWeight.tare > 0
          ? this.dispatch.receivalWeight.gross -
            this.dispatch.receivalWeight.tare
          : this.dispatch.receivalWeight.gross;
      const dispatched =
        this.dispatch.receivalWeight.tare > 0
          ? this.dispatch.initialWeight.gross - this.dispatch.initialWeight.tare
          : this.dispatch.initialWeight.gross;
      return (
        received -
        dispatched +
        " kg (" +
        Math.round(
          ((received - dispatched) / dispatched + Number.EPSILON) * 10000
        ) /
          100 +
        "%)"
      );
    },
  },
};
</script>