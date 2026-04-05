<template>
  <modal :showing="dispatch != null" @close="toggle" :showClose="true" :xl="true">
    <div id="ticket" style="margin-right: 10px; margin-left: 10px; margin-bottom: 25px">
      <div style="display: flex; justify-content: center" v-if="printImages">
        <img :src="logo.value" alt="" style="width: 200px; height: 73px" />
        <img src="../assets/meridian.jpg" alt="" style="width: 200px; height: 73px; margin-left: 20px" />
      </div>
      <div>
        <h2 style="text-align: center">
          {{
            (dispatch.status == "Processed" || dispatch.status == "Temp") &&
            dispatch.receivalWeight.printed
            ? "DUPLICATE"
            : dispatch.status == "Transit" && dispatch.initialWeight.printed
              ? "DUPLICATE"
              : ""
          }}
          WEIGHBRIDGE TICKET
        </h2>
      </div>
      <div style="width: 30%; height: 60px; display: table; margin-left: auto">
        <h4>Ticket Number</h4>
        <h3 style="font-size: 1.5rem">
          {{
            dispatch.status == "Processed" || dispatch.status == "Temp"
            ? dispatch.receivalWeight.ticketNumber ??
            (dispatch.cid > 0 ? dispatch.cid : dispatch.id + "*")
            : dispatch.initialWeight.ticketNumber ??
            (dispatch.cid > 0 ? dispatch.cid : dispatch.id + "*")
          }}
        </h3>
      </div>
      <table style="width: 100%">
        <tr>
          <td colspan="2" rowspan="8" style="vertical-align: top; text-align: center">
            <p>
              {{
                currentLocation.value?.name ?? "MFC"
              }}
            </p>
            <p>{{ currentLocation.value?.country?.name }}</p>
            <p>
              Telephone:
              {{
                currentLocation.value?.contacts ?? "N/A"
              }}
            </p>
            <p>{{ currentLocation.value?.website }}</p>
          </td>
          <td>Driver:</td>
          <td>{{ dispatch.booking?.driverName }}</td>
        </tr>
        <tr>
          <td>Drivers License Number:</td>
          <td>{{ dispatch.booking?.passportNumber }}</td>
        </tr>
        <tr>
          <td>Number Plate:</td>
          <td>{{ dispatch.booking?.numberPlate }}</td>
        </tr>
        <tr>
          <td>
            {{ dispatch.booking?.trailerNumber ? "Trailer Number:" : "" }}
          </td>
          <td>{{ dispatch.booking?.trailerNumber }}</td>
        </tr>
        <tr>
          <td>Transporter:<br /><br /></td>
          <td>{{ dispatch.booking?.transporter }}<br /><br /></td>
        </tr>

        <tr>
          <td>Destination:</td>
          <td>{{ dispatch.toOffice?.name }}</td>
        </tr>
        <tr>
          <td>
            {{ dispatch.booking?.branch?.name ? "&nbsp;&nbsp;Branch:" : "" }}
          </td>
          <td>{{ dispatch.booking?.branch?.name ?? "" }}</td>
        </tr>
        <tr>
          <td>Departure:<br /><br /></td>
          <td>{{ dispatch.fromOffice?.name }}<br /><br /></td>
        </tr>
        <tr>
          <td>Contract number:</td>
          <td>
            {{ dispatch.booking?.transportInstruction?.contract?.contractNumber }}
            <span v-if="dispatch.booking.tiChanges.length > 0" class="line-through">({{ prevTI.contract.contractNumber
            }})</span>
          </td>
          <td>Product Name:</td>
          <td style="max-width: 200px">
            {{ dispatch.product?.name }}
            <span v-if="dispatch.booking.tiChanges.length > 0" class="line-through">({{ prevTI.product.name }})</span>
          </td>
        </tr>
        <tr>
          <td>T.I Number:</td>
          <td>
            {{
              dispatch.booking?.transportInstruction?.kineticRef ??
              dispatch.booking?.transportInstruction?.id
            }}
          </td>
          <td>
            <span v-if="dispatch.booking.tiChanges.length > 0">
              Updated By:</span><br v-else />
          </td>
          <td>
            <span v-if="dispatch.booking.tiChanges.length > 0">
              {{
                dispatch.booking.tiChanges[
                  dispatch.booking.tiChanges.length - 1
                ]?.user.name
              }}</span><br v-else />
          </td>
        </tr>
        <tr>
          <td>
            <span v-if="dispatch.booking.loadingAuthorityNumber">Loading Authority Number</span>
            <span v-else-if="dispatch.booking.deliveryNoteNumber">Delivery Note Number</span>
          </td>
          <td>
            <span v-if="dispatch.booking.loadingAuthorityNumber">{{ dispatch.booking.loadingAuthorityNumber }}</span>
            <span v-else-if="dispatch.booking.deliveryNoteNumber">{{ dispatch.booking.deliveryNoteNumber }}</span>
          </td>
        </tr>
        <tr>
          <td><br></td>
        </tr>
        <tr>
          <td style="">Tare Mass</td>
          <td style="text-decoration: underline">
            {{
              isDispatching
              ? dispatch.initialWeight.tare
              : dispatch.receivalWeight.tare
            }}
            kg
          </td>
          <td>Nett Mass</td>
          <td style="text-decoration-line: underline">
            {{
              isDispatching
              ? dispatch.initialWeight.gross - dispatch.initialWeight.tare
              : dispatch.receivalWeight.gross - dispatch.receivalWeight.tare
            }}
            kg
          </td>
        </tr>

        <tr>
          <td style="">Gross Mass</td>
          <td style="text-decoration: underline">
            {{
              isDispatching
              ? dispatch.initialWeight.gross
              : dispatch.receivalWeight.gross
            }}
            kg
          </td>
          <td>
            {{
              !isDispatching && dispatch.status != "Temp" && dispatch.receivalWeight.tare > 0
              ? "1st Nett Mass"
              : ""
            }}
          </td>
          <td style="text-decoration-line: underline">
            {{
              !isDispatching && dispatch.status != "Temp" && dispatch.receivalWeight.tare > 0
              ? dispatch.initialWeight.gross -
              dispatch.initialWeight.tare +
              " kg"
              : ""
            }}
          </td>
        </tr>
        <tr v-if="dispatch.initialWeight.tare > 0 && dispatch.initialWeight.bags">
          <td>Dispatched:</td>
          <td>
            {{
              `${dispatch.initialWeight.bags ?? 300} Bags (${dispatch.initialWeight.size ?? 50
                }kg)`
            }}
          </td>
        </tr>
        <tr v-if="dispatch.receivalWeight.tare > 0 && dispatch.receivalWeight.bags
            ">
          <td>Received:</td>
          <td>
            {{
              `${dispatch.receivalWeight.bags ?? 300} Bags (${dispatch.receivalWeight.size ?? 50
                }kg)`
            }}
          </td>
        </tr>
      </table>
      <div style="padding: 2rem 0; margin-top: 3rem">
        Comments: .............................................
      </div>
      <div v-if="dispatch.initialWeight.grossUser || dispatch.receivalWeight.grossUser
          ">
        Operator Signature: ........................ Driver
        Signature:........................
        <br />
        Operator[ 1st Mass:
        {{
          isDispatching
          ? dispatch.initialWeight.tareUser?.name
          : dispatch.receivalWeight.grossUser?.name
        }}
        / 2nd Mass:
        {{
          isDispatching
          ? dispatch.initialWeight.grossUser?.name
          : dispatch.receivalWeight.tareUser?.name
        }}
        ]
        <br />
        In:
        {{
          dateFormat(
            isDispatching
            ? dispatch.initialWeight.tareAt
            : dispatch.receivalWeight.grossAt
          )
        }}
        / Out:
        {{
          dateFormat(
            isDispatching
            ? dispatch.initialWeight.grossAt
            : dispatch.receivalWeight.tareAt
          )
        }}
      </div>
    </div>
    <button v-if="isServer" v-print="'#ticket'" class="
        ml-4
        mb-4
        py-1
        px-2
        rounded
        shadow
        bg-accent
        hover:bg-primary
        text-white
      ">
      Print
    </button>
  </modal>
</template>

<script>
import Modal from "./Modal.vue";
import print from "vue3-print-nb";

export default {
  directives: {
    print,
  },
  components: { Modal },
  props: ["dispatch", "toggle"],
  inject: ["logo", "currentLocation"],
  mounted() {
    console.log(this.currentLocation);
  },
  computed: {
    printImages: () => import.meta.env.VITE_IMAGES == "true",
    isServer: () => import.meta.env.VITE_SERVER == "true",
    prevTI() {
      return this.dispatch.booking.tiChanges[
        this.dispatch.booking.tiChanges.length - 1
      ].oldValue;
    },
    isDispatching() {
      return this.currentLocation.value?.id == this.dispatch.booking.transportInstruction.fromLocation.id
    }
  },
  methods: {
    async print() {
      await this.$htmlToPaper("ticket");
    },
    dateFormat(date) {
      var options = {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
      };
      var today = new Date(date);

      return today.toLocaleDateString("en-US", options);
    },
  },
};
</script>
<style scoped>
#ticket {
  font-family: Courier;
  font-size: 14px;
  font-weight: 600;
}
</style>